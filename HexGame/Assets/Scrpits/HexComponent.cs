using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexComponent : MonoBehaviour {

    public Hex Hex;
    public HexMap HexMap;

	// Use this for initialization
	void Start () {
		
	}

	public void UpdatePosition () {
        this.transform.position = Hex.PositionFromCamera(Camera.main.transform.position,
            HexMap.Rows,
            HexMap.Columns,
            HexMap.horizWrapping,
            HexMap.vertWrapping
        );
	}
}
