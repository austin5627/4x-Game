using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building {

    public string Name;
    public string Desc;
    public int production;
    public int productionCost;
    public int productionRemaining;
    public int food;
    public int gold;
    public int science;
    public Research research;

    public Building(string Name, string Desc, int production, int productionCost, int food, int gold, int science, Research research)
    {
        this.Name = Name;
        this.Desc = Desc;
        this.production = production;
        this.productionCost = productionCost;
        this.productionRemaining = productionCost;
        this.food = food;
        this.gold = gold;
        this.science = science;
        this.research = research;
    }
    public Building(Building type)
    {
        this.Name = type.Name;
        this.Desc = type.Desc;
        this.production = type.production;
        this.productionCost = type.productionCost;
        this.productionRemaining = productionCost;
        this.food = type.food;
        this.gold = type.gold;
        this.science = type.science;
        this.research = type.research;
    }


}
