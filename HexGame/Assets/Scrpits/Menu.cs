using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

    public bool open;

	// Use this for initialization
	void Start () {
		
	}

    public void Quit()
    {
        Application.Quit();
    }
    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }
    public void Options()
    {
        open = false;
        gameObject.SetActive(false);


    }
}
