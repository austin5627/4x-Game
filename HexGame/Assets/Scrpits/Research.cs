using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Research{

    public int Science;
    public int ScienceRemaining;
    public int Level;
    public string Name;
    public Research[] Prerecs;

    public Research(int Science, string Name, Research[] Prerecs, int Level)
    {
        this.Science = Science;
        this.ScienceRemaining = Science;
        this.Level = Level;
        this.Name = Name;
        this.Prerecs = Prerecs;
    }

    public Research(Research r)
    {
        this.Science = r.Science;
        this.ScienceRemaining = r.Science;
        this.Name = r.Name;
        this.Prerecs = r.Prerecs;
        this.Level = r.Level;
    }

    public void DoScience(int science)
    {
        ScienceRemaining -= science;
    }
}
