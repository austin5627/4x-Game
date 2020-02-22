using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Year : MonoBehaviour {

    public void Start()
    {
        UpdateTurn(1, -753);
    }
    public void UpdateTurn(int turn, int year)
    {
        this.GetComponent<Text>().text = "Turn:" + turn + "\n";

        if (year >= 0)
        {
            this.GetComponent<Text>().text += "Year:" + year + "AD";
        }
        else
        {
            this.GetComponent<Text>().text += "Year:" + year*-1 + "BC";
        }
    }

}
