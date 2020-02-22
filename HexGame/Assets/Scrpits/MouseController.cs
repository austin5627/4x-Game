using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class MouseController : MonoBehaviour {

    public enum MouseMode {Cameradrag, UnitSelected, None, CitySelected}
    public MouseMode mode = MouseMode.None;
    Vector3 LastMousePos;
    public Unit selectedUnit;
    public City selectedCity;
    HexMap map;
    LineRenderer line;
    UnitPanel unitPanel;
    GameObject hexinfo;
    Button[] buttons;
    public GameObject cityScreen;
    public GameObject BuildUnitButtonPrefab;
    public GameObject BuildBuildingButtonPrefab;
    public GameObject SelectionIndicator;
    public GameObject Research;
    public List<BuildBuildingButton> BBBList;

    // Use this for initialization
    void Start() {
        map = GameObject.FindObjectOfType<HexMap>();
        line = GameObject.FindObjectOfType<LineRenderer>();
        hexinfo = GameObject.Find("Hex Info");
        line.sortingOrder = 10;
        buttons = new Button[1];
        buttons[0] = GameObject.Find("Settle").GetComponent<Button>();


        unitPanel = GameObject.FindObjectOfType<UnitPanel>();
        unitPanel.gameObject.SetActive(false);
    }


    public void SetUpCityScreen()
    {
        foreach(Unit u in map.UnitTypes.Values)
        {
            GameObject ButtonGo = Instantiate(BuildUnitButtonPrefab, cityScreen.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform);
            ButtonGo.GetComponentInChildren<Text>().text = u.Name;
            ButtonGo.GetComponent<BuildUnitButton>().type = u;
            Texture2D tex = (Texture2D)Resources.Load("UnitIcons/" + u.Name);
            Sprite sp = Sprite.Create(tex, new Rect(new Vector3(0f, 0f), new Vector3(tex.width, tex.height)), Vector2.zero);
            ButtonGo.GetComponentInChildren<Icon>().gameObject.GetComponent<Image>().sprite = sp;
        }
        foreach(Building b in map.BuildingTypes.Values)
        {
            GameObject ButtonGo = Instantiate(BuildBuildingButtonPrefab, cityScreen.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform);
            ButtonGo.GetComponentInChildren<Text>().text = b.Name;
            ButtonGo.GetComponent<BuildBuildingButton>().type = b;
            Texture2D tex = (Texture2D)Resources.Load("BuildingIcons/" + b.Name);
            Sprite sp = Sprite.Create(tex, new Rect(new Vector3(0f, 0f), new Vector3(tex.width, tex.height)), Vector2.zero);
            ButtonGo.GetComponentInChildren<Icon>().gameObject.GetComponent<Image>().sprite = sp;
            BBBList.Add(ButtonGo.GetComponent<BuildBuildingButton>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedUnit != null && selectedUnit.IsDestroyed == true)
        {
            selectedUnit = null;
            mode = MouseMode.None;
            return;
        }
        if (mode != MouseMode.UnitSelected)
        {
            SelectionIndicator.SetActive(false);
            SelectionIndicator.transform.parent = null;
        }



        if (mode != MouseMode.UnitSelected)
        {
            unitPanel.gameObject.SetActive(false);
        }
        else
        {
            unitPanel.gameObject.SetActive(true);
            unitPanel.UpdateText(selectedUnit);

            Texture2D tex = (Texture2D)Resources.Load("UnitIcons/" + selectedUnit.Name);
            Sprite sp = Sprite.Create(tex, new Rect(new Vector3(0f, 0f), new Vector3(tex.width, tex.height)), Vector2.zero);
            GameObject.FindGameObjectWithTag("UnitIcon").GetComponent<Image>().sprite = sp;
        }

        if (mode != MouseMode.CitySelected)
        {
            cityScreen.SetActive(false);
            Research.SetActive(true);
        }


        if (!EventSystem.current.IsPointerOverGameObject())
        {

            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            float rayLen = (mouseRay.origin.y / mouseRay.direction.y);
            Vector3 hitPos = mouseRay.origin - mouseRay.direction * rayLen;

            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.gameObject.GetComponentInParent<HexComponent>() != null)
                {
                    Hex h = hit.transform.gameObject.GetComponentInParent<HexComponent>().Hex;
                    string s = "";
                    if (!h.FogOfWar)
                    {
                        s += h.Type;
                        s += ", ";
                        s += h.Biome;
                        if (h.Resource != null && map.Researched.Contains(h.Resource.ResearchNeeded))
                            s += ", " + h.Resource.Name;
                        s += "\nP:" + h.Production  + "/F:" +  h.Food;
                    }
                    else
                    {
                        s = "Unknown";
                    }
                    hexinfo.GetComponentInChildren<Text>().text = s;
                }
            }


            if (Input.GetMouseButtonDown(0))
            {
                mode = MouseMode.Cameradrag;
                LastMousePos = hitPos;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (Physics.Raycast(mouseRay, out hit))
                {
                    if (hit.transform.gameObject.GetComponentInParent<UnitView>() != null)
                    {
                        SelectUnit(hit.transform.gameObject.GetComponentInParent<UnitView>().Unit);
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<CityView>() != null)
                    {
                        SelectCity(hit.transform.gameObject.GetComponentInParent<CityView>().City);
                    }
                    else
                    {
                        mode = MouseMode.None;
                    }
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                line.positionCount = 0;
            }


            //Zoom
            float scrollAmount = -Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollAmount) > 0.01)
            {
                Vector3 diff = Camera.main.transform.position - hitPos;

                Vector3 p = Camera.main.transform.position;

                if (scrollAmount < 0 || p.y < 20)
                {
                    Camera.main.transform.Translate(diff * scrollAmount, Space.World);
                }

                p = Camera.main.transform.position;

                if (p.y > 20)
                    p.y = 20;
                if (p.y < 2)
                    p.y = 2;
                Camera.main.transform.position = p;

            }

            if (mode == MouseMode.Cameradrag)
            {
                Update_MouseModeDrag();
                return;
            }
            else if (mode == MouseMode.UnitSelected)
            {
                Update_MouseModeUnit();
                return;
            }

        }
        if (mode == MouseMode.CitySelected)
        {
            Update_MouseModeCity();
            return;
        }
    }


    public void Update_MouseModeUnit()
    {

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (selectedUnit == null)
        {
            Debug.LogError("Selected Unit is null when in Update_MouseModeUnit()");
            mode = MouseMode.None;
            return;
        }

        UpdateUnitButtons();

        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.gameObject.GetComponentInParent<HexComponent>() != null)
                {
                    Hex h = hit.transform.gameObject.GetComponentInParent<HexComponent>().Hex;
                    Queue<Hex> q = Pathfinding.FindPath(h.map, selectedUnit.Hex, h);
                    if (q != null) {
                        Vector3[] arr = new Vector3[q.Count];
                        int i = 0;
                        while (q.Count > 0)
                        {
                            Hex hex = q.Dequeue();
                            Vector3 pos = hex.map.HexToGo[hex].transform.position;
                            pos.y += .1f;
                            arr[i] = pos;
                            i++;
                        }
                        line.positionCount = arr.Length;
                        line.SetPositions(arr);
                        line.sortingLayerName = "Line";
                        line.sortingOrder = 10;
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {

            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.gameObject.GetComponentInParent<HexComponent>() != null)
                {
                    Hex h = hit.transform.gameObject.GetComponentInParent<HexComponent>().Hex;
                    selectedUnit.SetPath(h);
                }
            }
        }
    }

    public void UpdateUnitButtons()
    {
        if (selectedUnit.CanSettle)
        {
            buttons[0].gameObject.SetActive(true);
        }
        else
        {
            buttons[0].gameObject.SetActive(false);
        }
    }

    public void Update_MouseModeCity()
    {
        if (selectedCity == null)
        {
            Debug.LogError("Selected City is Null when in Update_MouseModeCity");
            return;
        }
        cityScreen.GetComponentInChildren<Image>().gameObject.GetComponentInChildren<Text>().text = "CityName: " + selectedCity.Name +
            "\nProduction:" + selectedCity.TotalProduction +
            "\nFood:" + selectedCity.Food +
            "\nGold:" + selectedCity.Gold +
            "\nScience:" + selectedCity.Science + 
            "\nCulture:" + selectedCity.Culture;
        if (selectedCity.UnitBeingProduced != null)
        {
            cityScreen.GetComponentInChildren<Image>().gameObject.GetComponentInChildren<Text>().text += "\nProducing: " + selectedCity.UnitBeingProduced.Name + ": " + selectedCity.UnitBeingProduced.ProdutionRemaining + "/" + selectedCity.UnitBeingProduced.ProductionCost;
        }
        if (selectedCity.BuildingBeingProduced != null)
        {
            cityScreen.GetComponentInChildren<Image>().gameObject.GetComponentInChildren<Text>().text += "\nProducing: " + selectedCity.BuildingBeingProduced.Name + ": " + selectedCity.BuildingBeingProduced.productionRemaining + "/" + selectedCity.BuildingBeingProduced.productionCost;
        }


        foreach (BuildBuildingButton bbb in BBBList)
        {
            bbb.gameObject.SetActive(true);
            if (map.Researched.Contains(bbb.type.research)) { 
                foreach (Building b in selectedCity.Buildings)
                {
                    if (b.Name.Equals(bbb.type.Name))
                        bbb.gameObject.SetActive(false);
                }

            }
            else {
                bbb.gameObject.SetActive(false);
            }

        }




    }

    public void Update_MouseModeDrag()
    {
        if (Input.GetMouseButtonUp(0))
        {
            mode = MouseMode.None;
        }

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        float rayLen = (mouseRay.origin.y / mouseRay.direction.y);
        Vector3 hitPos = mouseRay.origin - mouseRay.direction * rayLen;

        Vector3 diff = LastMousePos - hitPos;
        Camera.main.transform.Translate(diff, Space.World);

        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        rayLen = (mouseRay.origin.y / mouseRay.direction.y);
        hitPos = mouseRay.origin - mouseRay.direction * rayLen;
    }

    public void SelectUnit(Unit unit)
    {
        selectedUnit = unit;
        mode = MouseMode.UnitSelected;
        SelectionIndicator.transform.parent = map.UnitToGo[selectedUnit].transform;
        SelectionIndicator.SetActive(true);
        SelectionIndicator.transform.position = map.UnitToGo[selectedUnit].transform.position;
        UpdateUnitButtons();
    }
    public void SelectCity(City city)
    {
        cityScreen.SetActive(true);
        Research.SetActive(false);
        mode = MouseMode.CitySelected;
        selectedCity = city;
    }


    public void SettleCity()
    {
        if (selectedUnit != null && selectedUnit.CanSettle)
        {
            if (map.SpawnCityAt(new City("City" + (map.Cities.Count + 1), map.GetHexAt(selectedUnit.Hex.Q, selectedUnit.Hex.R)), map.CityPrefab, selectedUnit.Hex.Q, selectedUnit.Hex.R))
            {
                SelectionIndicator.transform.parent = null;
                selectedUnit.Destroy();
            }
        }
        else {
            Debug.LogError("Selected untit is null or cant settle cities");
        }
    }
    public void DoNothing()
    {
        if(selectedUnit == null)
        {
            Debug.LogError("Selected Unit is Null");
            return;
        }
        selectedUnit.waiting = true;
    }

    public void SetProductionUnit(Unit type)
    {
        if (selectedCity == null)
        {
            Debug.LogError("Selected City is Null but city screen is up");
        }

        selectedCity.UnitBeingProduced = new Unit(type);
        selectedCity.BuildingBeingProduced = null;
    }
    public void SetProductionBuilding(Building type)
    {
        if (selectedCity == null)
        {
            Debug.LogError("Selected City is Null but city screen is up");

        }

        selectedCity.BuildingBeingProduced = new Building(type);
        selectedCity.UnitBeingProduced = null;
    }

    public void Sleep()
    {

        if (selectedUnit == null)
        {
            Debug.LogError("Selected Unit is Null");
            return;
        }
        selectedUnit.sleeping = true;
    }


}
