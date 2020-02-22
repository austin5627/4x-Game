using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour {


    float movepercent;
    Vector3 oldPos = Vector3.zero;
    [System.NonSerialized] public bool animating = false;
    [System.NonSerialized] public Unit Unit;

    public IEnumerator setHex(GameObject hex)
    {
        animating = true;
        movepercent = 0f;
        oldPos = this.transform.position;
        
        Vector3 newPos = oldPos;
        while(Unit.IsDestroyed == false && this.transform.position != hex.transform.position)
        {
            newPos.x = Mathf.Lerp(oldPos.x, hex.transform.position.x, movepercent);
            newPos.z = Mathf.Lerp(oldPos.z, hex.transform.position.z, movepercent);
            movepercent += Time.deltaTime;
            this.transform.position = newPos;
            yield return null;
        }
        
        if(this.transform.position == hex.transform.position)
        {
            this.transform.parent = hex.transform;
            animating = false;
        }

    }


}
