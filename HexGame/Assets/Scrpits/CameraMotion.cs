using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour {


    Vector3 oldPosition;

	// Use this for initialization
	void Start () {
        oldPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        CheckIfCameraMoved();
	}


    void CheckIfCameraMoved()
    {
        if (oldPosition != this.transform.position)
        {
            oldPosition = this.transform.position;
            HexComponent[] hexes = GameObject.FindObjectsOfType<HexComponent>();

            foreach (HexComponent Hex in hexes)
            {
                Hex.UpdatePosition();
            }

        }

    }
}
