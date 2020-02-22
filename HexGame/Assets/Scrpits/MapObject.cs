using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapObject {

    public Hex Hex { get; set; }

    public bool IsDestroyed { get; set; }

    public string Name;
    public int HitPoints = 100;
    public bool Attackable = true;
    public int Faction = 0;

    public delegate void ObjectDestroyedDelegate(MapObject mo);
    public event ObjectDestroyedDelegate OnObjectDestroyed; 

    public MapObject(string name)
    {
        this.Name = name;
    }

    virtual public void Destroy()
    {
        if (OnObjectDestroyed != null)
        {
            OnObjectDestroyed(this);
        }
    }
    
    virtual public void SetHex(Hex newHex)
    {
        Hex oldHex = Hex;
        Hex = newHex;
    }

}
