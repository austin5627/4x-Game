using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyController : MonoBehaviour {

    public MouseController mc;
    public Menu menu;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(mc.mode == MouseController.MouseMode.UnitSelected)
        {
            UnitMode();
        }
        if(mc.mode == MouseController.MouseMode.CitySelected)
        {
            CityMode();
        }

        if (Input.GetKeyDown("escape"))
        {
            if (menu.open)
            {
                menu.Close();
            }
            else if (!menu.open)
            {
                menu.Open();
            }
        }
	}

    public void UnitMode()
    {
        if (Input.GetKeyDown("space"))
        {
            mc.DoNothing();
        }
        if (Input.GetKeyDown("f"))
        {
            mc.Sleep();
        }
        if (Input.GetKeyDown("b") && mc.selectedUnit.CanSettle)
        {
            mc.SettleCity();
        }

    }
    public void CityMode()
    {
    }

}
