using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchButton : MonoBehaviour {
    public Research type;
    HexMap map;

    // Use this for initialization
    void Start() {
        map = FindObjectOfType<HexMap>();
        this.GetComponent<Button>().onClick.AddListener(SetResearch);
    }
	
	// Update is called once per frame
	public void SetResearch () {
        map.SetResearch(type);
	}
}
