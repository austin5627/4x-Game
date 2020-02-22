using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Xml;
using System;

public class HexMap : MonoBehaviour
{

    public Hex[,] Hexes { get; private set; }
    public Dictionary<Hex, GameObject> HexToGo { get; protected set; }

    public List<City> Cities { get; private set; }
    public Dictionary<City, GameObject> CityToGo { get; protected set; }


    public Dictionary<string, Unit> UnitTypes { get; private set; }
    public List<Unit> Units { get; private set; }
    public Dictionary<Unit, GameObject> UnitToGo { get; protected set; }
    public Dictionary<string, GameObject> UnitPrefabs { get; private set; }

    public Dictionary<string, Building> BuildingTypes { get; private set; }
    public Dictionary<string, Resource> ResourceTypes { get; private set; }

    public Dictionary<string, Research> Researches;
    public List<Research> Researched;

    public GameObject HexPrefab;
    public GameObject ForestPrefab;
    public GameObject JunglePrefab;
    public GameObject ResourceFlatPrefab;
    public GameObject ResourceHillPrefab;

    public GameObject UnitPrefab;
    public GameObject CityPrefab;

    public GameObject NextTurnButton;

    public GameObject Research;
    public GameObject ResearchButtonPrefab;
    public GameObject ResearchScreen;
    public GameObject ResearchTree;

    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshMountain;

    public Material MatOcean;
    public Material MatPlains;
    public Material MatGrassland;
    public Material MatDesert;
    public Material MatMounitain;
    public Material MatFog;

    public Research currentResearch;

    [System.NonSerialized] public float HeightMountain = 1f;
    [System.NonSerialized] public float HeightHill = 0.6f;
    [System.NonSerialized] public float HeightFlat = 0.0f;

    [System.NonSerialized] public float MoistureJungle = .8f;
    [System.NonSerialized] public float MoistureForest = 0.2f;
    [System.NonSerialized] public float MoistureGrassland = -.2f;
    [System.NonSerialized] public float MoisturePlains = -.5f;

    public readonly int Columns = 60, Rows = 30;

    [System.NonSerialized] public int year = -763;
    [System.NonSerialized] public int turn = 1;

    [System.NonSerialized] public bool vertWrapping = false;
    [System.NonSerialized] public bool horizWrapping = true;

    public delegate void OnNextTurnDelegate();
    public event OnNextTurnDelegate OnNextTurn;

    public Dictionary<Research, ResearchButton> RBList;
    public Dictionary<Research, ResearchButton> RBList2;

    // Use this for initialization
    void Start()
    {
        GenerateMap();
    }

    public void SetUpResearchScreen()
    {
        foreach (Research r in Researches.Values)
        {
            GameObject ButtonGo = Instantiate(ResearchButtonPrefab, ResearchScreen.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform);
            ButtonGo.GetComponentInChildren<Text>().text = r.Name;
            ButtonGo.GetComponent<ResearchButton>().type = r;
            Texture2D tex = (Texture2D)Resources.Load("ResearchIcons/" + r.Name);
            Sprite sp = Sprite.Create(tex, new Rect(new Vector3(0f, 0f), new Vector3(tex.width, tex.height)), Vector2.zero);
            ButtonGo.GetComponentInChildren<Icon>().gameObject.GetComponent<Image>().sprite = sp;
            RBList.Add(r, ButtonGo.GetComponent<ResearchButton>());
        }
    }

    public virtual void GenerateMap()
    {
        Units = new List<Unit>();
        Cities = new List<City>();
        Hexes = new Hex[Columns, Rows];
        HexToGo = new Dictionary<Hex, GameObject>();
        UnitToGo = new Dictionary<Unit, GameObject>();
        CityToGo = new Dictionary<City, GameObject>();
        UnitPrefabs = new Dictionary<string, GameObject>();
        RBList = new Dictionary<Research, ResearchButton>();
        RBList2 = new Dictionary<Research, ResearchButton>();
        Researched = new List<Research>();
        Researches = LoadResearch("Files/ResearchList");
        UnitTypes = LoadUnits("Files/UnitList");
        BuildingTypes = LoadBuildings("Files/BuildingList");
        ResourceTypes = LoadResources("Files/ResourceList");
        SetUpResearchScreen();
        SetUpResearchTree();
        FindObjectOfType<MouseController>().SetUpCityScreen();


        //Generates Water Map
        for (int col = 0; col < Columns; col++)
        {
            for (int row = 0; row < Rows; row++)
            {
                Hex h = new Hex(col, row, this);
                h.Elevation = -0.5f;

                Hexes[col, row] = h;

                GameObject hexGO = (GameObject)Instantiate(HexPrefab, h.PositionFromCamera(Camera.main.transform.position, Rows, Columns, horizWrapping, vertWrapping), Quaternion.identity, this.transform);

                HexToGo[h] = hexGO;

                hexGO.name = string.Format("HEX: ({0}, {1})", col, row);
                try
                {
                    hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", col, row);
                }
                catch { }
                hexGO.GetComponent<HexComponent>().Hex = h;
                hexGO.GetComponent<HexComponent>().HexMap = this;

            }
        }
        UpdateHexVisuals();
        GameObject.FindObjectOfType<Year>().UpdateTurn(turn, year);
        ToggleHexLabels();
    }

    public Hex GetHexAt(int q, int r)
    {

        if (Hexes == null)
        {
            Debug.LogError("Hex Array is Null");
            return null;
        }

        if (horizWrapping)
        {
            q = q % Columns;
            if (q < 0)
                q += Columns;
            if (q > Columns)
                q -= Columns;
        }
        if (vertWrapping)
        {
            r = r % Rows;
            if (r < 0)
                r += Rows;
            if (r > Rows)
                r -= Rows;
        }
        if (q >= Hexes.GetLength(0) || q < 0)
            return null;
        if (r >= Hexes.GetLength(1) || r < 0)
            return null;

        return Hexes[q, r];
    }

    public void SpawnUnitAt(string type, int q, int r)
    {

        if (!UnitTypes.ContainsKey(type))
        {
            Debug.LogError("There is no such thing as a " + type);
            return;
        }

        Unit UType = UnitTypes[type];
        Unit unit = new Unit(UType);

        unit.Hex = GetHexAt(q, r);
        unit.Hex.unit = unit;
        GameObject UnitGO = GameObject.Instantiate(UnitPrefabs[type], HexToGo[unit.Hex].transform);
        try
        {
            Mesh mesh = (Mesh)Resources.Load("Models/MapObjects/Units/" + type, typeof(Mesh));
            UnitGO.GetComponentInChildren<MeshFilter>().mesh = mesh;
            UnitGO.GetComponentInChildren<MeshCollider>().sharedMesh = mesh;
            //ONLY USES ONE MESH
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Mesh mesh = (Mesh)Resources.Load("Models/MapObjects/Units/Unit", typeof(Mesh));
            UnitGO.GetComponentInChildren<MeshFilter>().mesh = mesh;
            UnitGO.GetComponentInChildren<MeshCollider>().sharedMesh = mesh;
        }
        UnitGO.GetComponent<UnitView>().Unit = unit;
        UnitGO.name = unit.Name + " " + unit.Hex;
        UnitToGo.Add(unit, UnitGO);
        Units.Add(unit);
        unit.OnUnitMoved += UpdateFogOfWar;
        unit.OnUnitMoved += unit.ClearSleep;
        UpdateFogOfWar(unit);
        unit.OnObjectDestroyed += DesroyUnit;
    }
    public bool SpawnCityAt(City city, GameObject prefab, int q, int r)
    {
        Hex[] hs = GetHexesInRadius(GetHexAt(q, r), 4);
        foreach (Hex h in hs)
        {
            if (h.city != null)
            {
                return false;
            }
        }
        GetHexAt(q, r).city = city;
        city.Hex = GetHexAt(q, r);

        if (city.Hex.Biome == Hex.HexBiome.Forest || city.Hex.Biome == Hex.HexBiome.Jungle)
        {
            city.Hex.Biome = Hex.HexBiome.Grassland;
            city.Hex.Moisture = MoistureGrassland;
            UpdateHexVisuals();
        }

        GameObject CityGO = GameObject.Instantiate(prefab, HexToGo[city.Hex].transform);
        if (city.Hex.Type == Hex.HexType.Hill)
        {
            Vector3 pos = CityGO.transform.position;
            pos.y += .2f;
            CityGO.transform.position = pos;
        }

        CityGO.name = city.Name + " " + city.Hex;
        city.Buildings.Add(BuildingTypes["Palace"]);
        CityToGo.Add(city, CityGO);
        Cities.Add(city);
        CityGO.GetComponent<CityView>().City = city;
        city.hexes = GetHexesInRadius(city.Hex, 2).ToList();
        UpdateFogOfWar(new Unit("", GetHexAt(q, r)) { Vision = 4 });


        return true;
    }

    public void DesroyUnit(Unit u)
    {
        GameObject unitGO = UnitToGo[u];
        GameObject.Destroy(unitGO);
        UnitToGo.Remove(u);
        Units.Remove(u);
    }
    public void ClearFogOfWar()
    {
        foreach (Hex h in Hexes)
        {
            h.FogOfWar = false;
        }
        UpdateHexVisuals();
    }

    bool labels = false;
    public void ToggleHexLabels()
    {
        foreach (Hex h in Hexes)
        {
            if (labels)
            {
                HexToGo[h].GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", h.Q, h.R);
            }
            else
            {
                HexToGo[h].GetComponentInChildren<TextMesh>().text = "";
            }
        }

        if (labels)
        {
            labels = false;
        }
        else
        {
            labels = true;
        }
    }

    public void Update()
    {
        UpdateResearch();
        UpdateUnitMovement();
        UpdateTurnButton();

    }

    public void UpdateResearch()
    {
        if (currentResearch == null)
        {
        }
        else
        {
            GameObject rI = Research.transform.Find("ResearchCircles").Find("ResearchIcon").gameObject;
            Text txt = Research.GetComponentInChildren<Text>();
            txt.text = currentResearch.Name + ":" + (currentResearch.Science - currentResearch.ScienceRemaining) + "/" + currentResearch.Science;
            Texture2D tex = (Texture2D)Resources.Load("ResearchIcons/" + currentResearch.Name);
            Sprite sp = Sprite.Create(tex, new Rect(new Vector3(0f, 0f), new Vector3(tex.width, tex.height)), Vector2.zero);
            rI.GetComponentInChildren<Image>().sprite = sp;
            //FIX ME: Change to global Science
        }
        if (currentResearch != null && currentResearch.ScienceRemaining <= 0)
        {
            Researched.Add(currentResearch);
            RBList2[currentResearch].GetComponent<Button>().onClick.RemoveAllListeners();
            UpdateHexVisuals();
            currentResearch = null;
        }
        foreach (Research r in Researches.Values)
        {
            RBList[r].gameObject.SetActive(true);
            RBList2[r].GetComponent<Image>().color = Color.white;
            RBList2[r].GetComponent<Button>().onClick.AddListener(RBList2[r].GetComponent<ResearchButton>().SetResearch);
            if (Researched.Contains(r))
            {
                RBList[r].gameObject.SetActive(false);
                RBList2[r].GetComponent<Image>().color = new Color(255, 255, 0);
                continue;
            }
            foreach (Research pre in r.Prerecs)
            {
                if (!Researched.Contains(pre))
                {
                    RBList[r].gameObject.SetActive(false);
                    RBList2[r].GetComponent<Image>().color = Color.grey;
                    RBList2[r].GetComponent<Button>().onClick.RemoveAllListeners();
                    break;
                }

            }
        }

    }

    public void UpdateUnitMovement()
    {
        foreach (Unit u in Units)
        {
            if (!UnitToGo[u].GetComponent<UnitView>().animating && u.movementRemaining > 0)
            {
                Hex next = u.NextHex();
                if (next != null)
                {
                    if (next.unit == null)
                    {
                        StartCoroutine(UnitToGo[u].GetComponent<UnitView>().setHex(HexToGo[next]));
                        u.SetHex(next);
                    }
                    else
                    {
                        Debug.Log("NEXT CONTAINS UNIT?" + next.unit);
                        u.SetPath(u.Hex);
                    }
                }
            }
        }

    }
    public void UpdateTurnButton()
    {
        foreach (City c in Cities)
        {
            if (c.UnitBeingProduced == null && c.BuildingBeingProduced == null)
            {
                NextTurnButton.GetComponent<Button>().onClick.RemoveAllListeners();
                NextTurnButton.GetComponent<Button>().onClick.AddListener(delegate { SelectCity(c); });
                NextTurnButton.GetComponentInChildren<Text>().text = "Choose\nProduction";
                NextTurnButton.GetComponent<Image>().color = new Color(255f, 200f, 0, 255f);
                return;
            }
        }
        foreach (Unit u in Units)
        {
            if (u.movementRemaining > 0 && !u.waiting && !u.sleeping)
            {
                NextTurnButton.GetComponent<Button>().onClick.RemoveAllListeners();
                NextTurnButton.GetComponent<Button>().onClick.AddListener(delegate { SelectUnit(u); });
                NextTurnButton.GetComponentInChildren<Text>().text = "Unit\nNeeds\nOrders";
                NextTurnButton.GetComponent<Image>().color = new Color(0f, 255, 0f, 255f);
                return;
            }
        }
        if ((currentResearch == null || currentResearch.ScienceRemaining <= 0) && (Researches.Count != Researched.Count))
        {
            NextTurnButton.GetComponent<Button>().onClick.RemoveAllListeners();
            NextTurnButton.GetComponent<Button>().onClick.AddListener(SelectResearch);
            NextTurnButton.GetComponentInChildren<Text>().text = "Choose\nResearch";
            NextTurnButton.GetComponent<Image>().color = new Color(0f, 255f, 255f, 255f);
            return;
        }


        NextTurnButton.GetComponent<Button>().onClick.RemoveAllListeners();
        NextTurnButton.GetComponent<Button>().onClick.AddListener(NextTurn);
        NextTurnButton.GetComponentInChildren<Text>().text = "Next Turn";
        NextTurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 255);

    }

    //Next Turn Button Methods
    public void SelectUnit(Unit unit)
    {
        MouseController mouseController = FindObjectOfType<MouseController>();
        mouseController.SelectUnit(unit);

        ZoomCameraToGameObject(UnitToGo[unit]);

    }
    public void SelectCity(City city)
    {
        MouseController mouseController = FindObjectOfType<MouseController>();
        mouseController.SelectCity(city);

        ZoomCameraToGameObject(CityToGo[city]);

    }
    public void SelectResearch()
    {
        ResearchScreen.SetActive(true);
    }
    public void NextTurn()
    {
        foreach (GameObject uGo in UnitToGo.Values)
        {
            if (uGo.GetComponent<UnitView>().animating)
            {
                return;
            }
        }

        foreach (Unit u in Units)
        {
            u.movementRemaining = u.Movement;
            u.waiting = false;
        }

        foreach (City c in Cities)
        {
            c.DoTurn();
            c.Grow(this);
        }
        year += 10;
        turn += 1;
        GameObject.FindObjectOfType<Year>().UpdateTurn(turn, year);

        GameObject rP = Research.gameObject.transform.Find("ResearchCircles").Find("ResearchProgress").gameObject;

        if (currentResearch != null)
        {
            currentResearch.ScienceRemaining -= 5;
            float fA = (float)(currentResearch.Science - currentResearch.ScienceRemaining) / (float)currentResearch.Science;
            rP.GetComponent<Image>().fillAmount = fA;
        }
        OnNextTurn();

    }

    public void ZoomCameraToGameObject(GameObject go)
    {
        Vector3 pos = new Vector3(go.transform.position.x, 5, go.transform.position.z - 3);
        Camera.main.transform.position = pos;
    }


    public void UpdateFogOfWar(Unit unit)
    {
        Hex[] hexes = GetHexesInRadius(unit.Hex, unit.Vision);
        foreach (Hex h in hexes)
        {
            if (h != null)
                h.FogOfWar = false;
        }
        UpdateHexVisuals();
    }


    public void ToggleResearchScreen()
    {
        if (ResearchScreen.activeSelf)
        {
            ResearchScreen.SetActive(false);
        }
        else
        {
            ResearchScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Reads Unit Types From XML File
    /// </summary>
    /// <param name="path">The Location of the XML File</param>
    /// <returns>A Dictionary of Unit Names and Unit Types</returns>
    public Dictionary<string, Unit> LoadUnits(string path)
    {
        Dictionary<string, Unit> types = new Dictionary<string, Unit>();
        XmlDocument doc = new XmlDocument();
        string xml = Resources.Load(path).ToString();
        doc.LoadXml(xml);
        foreach (XmlNode node in doc.DocumentElement["Units"])
        {
            string name = node["Name"].InnerText;
            int Movement = int.Parse(node["Movement"].InnerText);
            int Vision = int.Parse(node["Vision"].InnerText);
            bool CanSettle = bool.Parse(node["CanSettle"].InnerText);
            int ProductionCost = int.Parse(node["ProductionCost"].InnerText);
            int HitPoints = int.Parse(node["HitPoints"].InnerText);
            bool Attackable = bool.Parse(node["Attackable"].InnerText);
            Unit u = new Unit(name, Movement, Vision, CanSettle, ProductionCost, HitPoints, Attackable, GetHexAt(0, 0));
            //CHANGEME
            UnitPrefabs.Add(name, UnitPrefab);
            types.Add(name, u);
        }

        return types;
    }

    public Dictionary<string, Building> LoadBuildings(string path)
    {
        Dictionary<string, Building> types = new Dictionary<string, Building>();
        XmlDocument doc = new XmlDocument();
        string xml = Resources.Load(path).ToString();
        doc.LoadXml(xml);
        foreach (XmlNode node in doc.DocumentElement["Buildings"])
        {
            string name = node["Name"].InnerText;
            string desc = node["Desc"].InnerText;
            int Production = int.Parse(node["Production"].InnerText);
            int ProductionCost = int.Parse(node["ProductionCost"].InnerText);
            int Food = int.Parse(node["Food"].InnerText);
            int Gold = int.Parse(node["Gold"].InnerText);
            int Science = int.Parse(node["Science"].InnerText);
            Research research = Researches[node["Research"].InnerText];
            Building b = new Building(name, desc, Production, ProductionCost, Food, Gold, Science, research);
            types.Add(name, b);
        }

        return types;
    }


    public Dictionary<string, Resource> LoadResources(string path)
    {
        Dictionary<string, Resource> types = new Dictionary<string, Resource>();
        XmlDocument doc = new XmlDocument();
        string xml = Resources.Load(path).ToString();
        doc.LoadXml(xml);
        foreach (XmlNode node in doc.DocumentElement["Resources"])
        {
            string name = node["Name"].InnerText;
            int Production = int.Parse(node["Production"].InnerText);
            int Food = int.Parse(node["Food"].InnerText);
            int Gold = int.Parse(node["Gold"].InnerText);
            int Science = int.Parse(node["Science"].InnerText);
            Research researchNeeded = Researches[node["ResearchNeeded"].InnerText];
            Resource r = new Resource(name, GetHexAt(0, 0), Production, Food, Gold, Science, researchNeeded);
            types.Add(name, r);
        }

        return types;
    }

    public Dictionary<string, Research> LoadResearch(string path)
    {
        Dictionary<string, Research> temp = new Dictionary<string, Research>();
        XmlDocument doc = new XmlDocument();
        string xml = Resources.Load(path).ToString();
        doc.LoadXml(xml);
        foreach (XmlNode node in doc.DocumentElement["Researches"])
        {

            string Name = node["Name"].InnerText;
            int Science = int.Parse(node["Science"].InnerText);
            string prerecs = node["Prerecs"].InnerText;
            int Level = int.Parse(node["Level"].InnerText);
            List<Research> rs = new List<Research>();
            if (!prerecs.Equals("none"))
            {
                string[] names = prerecs.Split(',');
                foreach (string n in names)
                {
                    rs.Add(temp[n]);
                }
            }
            Research r = new Research(Science, Name, rs.ToArray(), Level);
            temp.Add(r.Name, r);
        }
        Researched.Add(temp["Agriculture"]);
        return temp;
    }

    public void SetUpResearchTree()
    {
        foreach (Research r in Researches.Values)
        {
            GameObject level;
            if (ResearchTree.GetComponentInChildren<HorizontalLayoutGroup>().transform.Find("Level " + r.Level) == null)
            {

                level = new GameObject("Level " + r.Level);
                level.transform.parent = ResearchTree.GetComponentInChildren<HorizontalLayoutGroup>().transform;
                level.transform.localScale = Vector3.one;
                level.AddComponent<VerticalLayoutGroup>();
                level.AddComponent<ContentSizeFitter>().SetLayoutHorizontal();
                level.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
                level.GetComponent<VerticalLayoutGroup>().childControlWidth = false;
                level.GetComponent<VerticalLayoutGroup>().spacing = 30;
                level.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                level.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                //ICONS AND TEXT


            }
            else
            {
                level = ResearchTree.GetComponentInChildren<HorizontalLayoutGroup>().transform.Find("Level " + r.Level).gameObject;
            }

            GameObject ButtonGo = Instantiate(ResearchButtonPrefab, level.transform);
            ButtonGo.GetComponent<ResearchButton>().type = r;
            ButtonGo.GetComponentInChildren<Text>().text = r.Name;
            Texture2D tex = (Texture2D)Resources.Load("ResearchIcons/" + r.Name);
            Sprite sp = Sprite.Create(tex, new Rect(new Vector3(0f, 0f), new Vector3(tex.width, tex.height)), Vector2.zero);
            ButtonGo.GetComponentInChildren<Icon>().gameObject.GetComponent<Image>().sprite = sp;
            RBList2.Add(r, ButtonGo.GetComponent<ResearchButton>());
        }
    }
    public void OpenResearchTree()
    {
        ResearchTree.SetActive(true);
    }
    public void CloseResearchTree()
    {
        ResearchTree.SetActive(false);
    }



    public void SetResearch(Research r)
    {
        if (currentResearch != null)
            RBList2[currentResearch].GetComponent<Image>().color = new Color(255, 255, 255);

        currentResearch = r;
        RBList2[currentResearch].GetComponent<Image>().color = new Color(0, 255, 255, 255);
        ResearchScreen.SetActive(false);
    }

    //FIXME: Might not work
    public Hex[] GetHexesInRadius(Hex center, int radius)
    {
        List<Hex> results = new List<Hex>();

        for (int x = -radius; x < radius - 1; x++)
        {
            for (int y = Mathf.Max(-radius + 1, -x - radius); y < Mathf.Min(radius, -x + radius - 1); y++)
            {
                results.Add(GetHexAt(center.Q + x + 1, center.R + y));
            }
        }

        return results.ToArray();

    }


    public void UpdateHexVisuals()
    {

        for (int col = 0; col < Columns; col++)
        {
            for (int row = 0; row < Rows; row++)
            {
                Hex h = GetHexAt(col, row);
                GameObject hexGO = HexToGo[h];
                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();
                h.Food = 0;
                h.Production = 0;
                if (h.Resource != null && !h.FogOfWar)
                {

                    if (h.Type == Hex.HexType.Flat && hexGO.transform.Find("Resource") == null && Researched.Contains(h.Resource.ResearchNeeded))
                    {
                        GameObject re = Instantiate(ResourceFlatPrefab, hexGO.transform);
                        re.name = "Resource";
                        re.GetComponentInChildren<MeshRenderer>().material = (Material)Resources.Load(h.Resource.Name);
                    }
                    else if (h.Type == Hex.HexType.Hill && hexGO.transform.Find("Resource") == null && Researched.Contains(h.Resource.ResearchNeeded))
                    {
                        GameObject re = Instantiate(ResourceHillPrefab, hexGO.transform);
                        re.name = "Resource";
                        re.GetComponentInChildren<MeshRenderer>().material = (Material)Resources.Load(h.Resource.Name);
                    }
                    h.Production += h.Resource.Production;
                    h.Food += h.Resource.Food;
                    h.Gold += h.Resource.Gold;
                    h.Science += h.Resource.Science;
                    h.Culture += h.Resource.Culture;
                }


                if (h.Elevation >= HeightMountain)
                {
                    mr.material = MatMounitain;
                    mf.mesh = MeshMountain;
                    h.MovementCost = -1;
                    h.Type = Hex.HexType.Mountain;
                    h.Biome = Hex.HexBiome.Mountain;
                    h.Production = 0;
                    h.Food = 0;
                }
                else if (h.Elevation >= HeightHill)
                {
                    mf.mesh = MeshHill;
                    h.MovementCost = 2;
                    h.Type = Hex.HexType.Hill;
                    h.Production += 2;
                    h.Food += 0;
                }
                else if (h.Elevation >= HeightFlat)
                {
                    mf.mesh = MeshFlat;
                    h.MovementCost = 1;
                    h.Type = Hex.HexType.Flat;
                }
                else
                {
                    mf.mesh = MeshFlat;
                    mr.material = MatOcean;
                    h.MovementCost = -1;
                    h.Type = Hex.HexType.Ocean;
                    h.Production += 0;
                    h.Food += 2;
                }



                if (h.Elevation >= HeightFlat && h.Elevation < HeightMountain)
                {
                    if (h.Moisture >= MoistureJungle)
                    {
                        mr.material = MatGrassland;
                        Vector3 p = hexGO.transform.position;
                        if (h.Elevation <= HeightHill)
                        {
                            p.y -= 0.1f;
                        }
                        if (!h.FogOfWar && hexGO.transform.Find("Jungle") == null)
                        {
                            GameObject forest = GameObject.Instantiate(JunglePrefab, p, Quaternion.identity);
                            forest.transform.parent = hexGO.transform;
                            forest.name = "Jungle";
                            h.first = false;
                        }
                        h.MovementCost = 2;
                        h.Biome = Hex.HexBiome.Jungle;
                        h.Food += 2;
                        h.Production += 1;
                    }
                    else if (h.Moisture >= MoistureForest)
                    {
                        mr.material = MatGrassland;

                        Vector3 p = hexGO.transform.position;
                        if (h.Elevation <= HeightHill)
                        {
                            p.y -= 0.1f;
                        }
                        h.MovementCost = 2;
                        if (!h.FogOfWar && hexGO.transform.Find("Forest") == null)
                        {
                            GameObject forest = GameObject.Instantiate(ForestPrefab, p, Quaternion.identity);
                            forest.transform.parent = hexGO.transform;
                            forest.name = "Forest";
                            h.first = false;
                        }
                        h.Biome = Hex.HexBiome.Forest;
                        h.Food += 1;
                        h.Production += 2;
                    }
                    else if (h.Moisture >= MoistureGrassland)
                    {
                        mr.material = MatGrassland;
                        h.Biome = Hex.HexBiome.Grassland;
                        h.Production += 0;
                        h.Food += 2;
                    }
                    else if (h.Moisture >= MoisturePlains)
                    {
                        mr.material = MatPlains;
                        h.Biome = Hex.HexBiome.Plains;
                        h.Production += 0;
                        h.Food += 2;
                    }
                    else
                    {
                        mr.material = MatDesert;
                        h.Biome = Hex.HexBiome.Desert;
                        h.Production += 0;
                        h.Food += 1;
                    }
                }
                if (h.FogOfWar)
                {
                    mr.material = MatFog;
                    mf.mesh = MeshFlat;
                }
            }
        }
    }

}
