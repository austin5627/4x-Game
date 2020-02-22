using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MapObject {

    public int Movement = 2;
    public int movementRemaining;
    public Queue<Hex> Path { get; private set; }
    public bool CanSettle;
    public int Vision = 3;

    public delegate void OnUnitMovedDelegate(Unit unit);
    public event OnUnitMovedDelegate OnUnitMoved;

    public int ProductionCost;
    public int ProdutionRemaining;
    public bool waiting;
    public bool sleeping;

    public new delegate void ObjectDestroyedDelegate(Unit u);
    public new event ObjectDestroyedDelegate OnObjectDestroyed;

    public Unit(string name, Hex hex) : base(name)
    {
        this.Hex = hex;
        movementRemaining = Movement;
        ProdutionRemaining = ProductionCost;
    }

    public Unit(string name, int Movement, int Vision, bool CanSettle, int ProductionCost, int HitPoints, bool Attackable, Hex hex) : base(name)
    {
        this.Hex = hex;
        this.Movement = Movement;
        this.Vision = Vision;
        this.CanSettle = CanSettle;
        this.ProductionCost = ProductionCost;
        this.HitPoints = HitPoints;
        this.Attackable = Attackable;

        movementRemaining = Movement;
        ProdutionRemaining = ProductionCost;
    }

    public Unit(Unit u) : base(u.Name)
    {
        this.Hex = u.Hex;
        this.Movement = u.Movement;
        this.Vision = u.Vision;
        this.CanSettle = u.CanSettle;
        this.ProductionCost = u.ProductionCost;
        this.HitPoints = u.HitPoints;
        this.Attackable = u.Attackable;

        movementRemaining = Movement;
        ProdutionRemaining = ProductionCost;
    }

    public float MoventCostToEnterHex(Hex hex)
    {
        return hex.MovementCost;
    }


    public float AggregatTurnsToEnterHex(Hex hex, float turnsToDate)
    {
        float baseTurnsToEnterHex = MoventCostToEnterHex(hex) / Movement;


        // Hex costs more than our total movement
        if (baseTurnsToEnterHex > 1)
        {
            baseTurnsToEnterHex = 1;
        }

        float turnsToDateWhole = Mathf.Floor(turnsToDate);
        float turnsToDateFrac = turnsToDate - turnsToDateWhole;
        
        if (turnsToDateFrac < 0.01f || turnsToDateFrac > .99f)
        {
            if (turnsToDateFrac < 0.01f)
                turnsToDateFrac = 0;
            if(turnsToDateFrac > .99f)
            {
                turnsToDateFrac = 0;
                turnsToDateWhole += 1;
            }

        }

        float turnsUsedAfterThisMove = turnsToDateFrac + baseTurnsToEnterHex;

        //We have movement left but not enough to enter Tile
        //so we allow the unit some free movement points
        if(turnsUsedAfterThisMove > 1)
        {   
            turnsUsedAfterThisMove = 1;
        }

        return turnsToDateWhole + turnsUsedAfterThisMove;

    }


    public override void SetHex(Hex newHex)
    {
        Hex oldHex = this.Hex;
        if (oldHex != null)
        {
            oldHex.unit = null;
        }

        movementRemaining -= newHex.MovementCost;
        if (movementRemaining < 0)
        {
            movementRemaining = 0;  
        }

        Hex = newHex;

        Hex.unit = this;
        OnUnitMoved(this);
    }

    public void ClearSleep(Unit u)
    {
        u.sleeping = false;
    }

    public override void Destroy()
    {
        this.OnObjectDestroyed(this);
        IsDestroyed = true;
        Hex.unit = null;
    }
    
    public void SetPath(Hex dest)
    {
        Path = Pathfinding.FindPath(Hex.map, Hex, dest);
        if (Path != null)
            Path.Dequeue();
    }
    public Hex NextHex()
    {
        if (Path != null && Path.Count != 0)
            return Path.Dequeue();
        return null;
    }

}
