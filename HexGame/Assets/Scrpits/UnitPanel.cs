using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPanel : MonoBehaviour {
    
    

	// Update is called once per frame
	public void UpdateText (Unit unit) {
        if (unit == null)
            return;
        string text = "";
        text += unit.Name + "\n";
        text += "Movement:" + unit.movementRemaining + "/" + unit.Movement + "\n";
        text += "Health:" + unit.HitPoints;

        this.gameObject.GetComponentInChildren<Text>().text = text;
	}

}
