using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildBuildingButton : MonoBehaviour {
    public Building type;
    MouseController mouseController;


	// Use this for initialization
	void Start () {
        mouseController = FindObjectOfType<MouseController>();
        this.GetComponent<Button>().onClick.AddListener(SetBuildBuilding);
	}
	
	// Update is called once per frame
	void SetBuildBuilding () {
        mouseController.SetProductionBuilding(type);	
	}
}
