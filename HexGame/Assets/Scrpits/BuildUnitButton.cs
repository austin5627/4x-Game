using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUnitButton : MonoBehaviour {
    public Unit type;
    MouseController mouseController;

	// Use this for initialization
	void Start () {
        mouseController = FindObjectOfType<MouseController>();
        this.GetComponent<Button>().onClick.AddListener(SetBuildUnit);
	}
	
	// Update is called once per frame
	void SetBuildUnit() {
        mouseController.SetProductionUnit(type);
	}
}
