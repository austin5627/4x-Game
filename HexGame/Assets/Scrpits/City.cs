using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class City : MapObject {

    public Unit UnitBeingProduced;
    public List<Building> Buildings;
    public Building BuildingBeingProduced;
    public List<Hex> hexes;

    public int Population;

    public int BaseProduction = 2;
    public int TotalProduction;
    public int Food;
    public int Gold;
    public int Science;
    public int Culture;


    public City(string name, Hex hex) : base(name)
    {
        Buildings = new List<Building>();
        this.Hex = hex;
        UpdateResources();

    }

    public void DoTurn()
    {
        UpdateResources();
        Produce(TotalProduction);
        if (UnitBeingProduced != null && UnitBeingProduced.ProdutionRemaining <= 0)
        {
            GameObject.FindObjectOfType<HexMap>().SpawnUnitAt(UnitBeingProduced.Name, Hex.Q, Hex.R);
            UnitBeingProduced = null;
        }
        if (BuildingBeingProduced != null && BuildingBeingProduced.productionRemaining <= 0)
        {
            Buildings.Add(BuildingBeingProduced);
            UpdateResources();
            BuildingBeingProduced = null;
        }
    }

    public void UpdateResources()
    {
        TotalProduction = BaseProduction;
        TotalProduction += Hex.Production;
        Food = Hex.Food;
        Gold = Hex.Gold;
        Science = Hex.Science;
        Culture = Hex.Culture;

        foreach (Building b in Buildings)
        {
            TotalProduction += b.production;
            Gold += b.gold;
            Food += b.food;
            Science += b.science;
        }
    }

    public void Produce(int production)
    {
        if (UnitBeingProduced != null)
        {
            UnitBeingProduced.ProdutionRemaining -= production;
        }
        if (BuildingBeingProduced != null)
        {
            BuildingBeingProduced.productionRemaining -= production;
        }
    }

    public List<Hex> getNeighbors(HexMap map)
    {
        List<Hex> ns = new List<Hex>();
        List<Hex> edgeHexes = GetEdgeHexes(map);
        if (edgeHexes == null)
        {
            return null;
        }
        foreach (Hex e in edgeHexes)
        {
            foreach (Hex n in e.GetNieghbors(map))
            {
                if (hexes.Contains(n) || !map.GetHexesInRadius(Hex, 4).Contains(n))
                {
                    continue;
                }
                else
                    ns.Add(n);
            }
        }

        return ns;
    }

    public void Grow(HexMap map)
    {
        List<Hex> neighbors = getNeighbors(map);
        if(neighbors.Count > 0)
            hexes.Add(neighbors[Random.Range(0, neighbors.Count)]);
    }

    public List<Hex> GetEdgeHexes(HexMap map)
    {
        List<Hex> edges = new List<Hex>(); 
        if(hexes == null || hexes.Count() < 1)
        {
            return null;
        }


        foreach (Hex h in hexes)
        {
            foreach (Hex n in h.GetNieghbors(map))
            {
                if (hexes.Contains<Hex>(n))
                    continue;
                else
                {
                    edges.Add(h);
                }
            }
        }
        return edges;
    }


}
