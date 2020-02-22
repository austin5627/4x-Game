using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MapObject {

    public int Production;
    public int Gold;
    public int Food;
    public int Science;
    public int Culture;
    public Research ResearchNeeded;

    // Use this for initialization
    public Resource(string Name, Hex Hex, int Production, int Gold, int Food, int Science, Research ResearchNeeded) : base(Name)
    {
        this.Attackable = false;
        this.Hex = Hex;

        this.Production = Production;
        this.Gold = Gold;
        this.Science = Food;
        this.Science = Science;
        this.ResearchNeeded = ResearchNeeded;
    }
    public Resource(Resource r) : base(r.Name)
    {
        this.Attackable = false;
        this.Hex = Hex;

        this.Production = r.Production;
        this.Gold = r.Gold;
        this.Science = r.Food;
        this.Science = r.Science;
        this.ResearchNeeded = r.ResearchNeeded;
    }



}
