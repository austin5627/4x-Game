using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EventController : MonoBehaviour {
    public HexMap map;

    public Dictionary<int, GameEvent> events;

    public GameObject EventPanel;


	// Use this for initialization
	void Start () {
        this.map = GameObject.FindObjectOfType<HexMap>();
        this.events = new Dictionary<int, GameEvent>();
        EventPanel.gameObject.SetActive(false);
        map.OnNextTurn += CheckForEvent;
        Object[] e = Resources.LoadAll("Files/Events/");
        CreateEvents(e);
	}

    public void CloseEventPanel()
    {
        EventPanel.SetActive(false);
    }

    public void CheckForEvent()
    {
        if (events.ContainsKey(map.year))
        {
            GameEvent e = events[map.year];
            EventPanel.gameObject.SetActive(true);
            GameObject.Find("Event Text").GetComponent<Text>().text = e.text;
            GameObject.Find("Event Title").GetComponent<Text>().text = e.name;
        }
        else {
            CloseEventPanel();
        }
    }
    
    public void CreateEvents(Object[] eventList)
    {
        string name = "";
        string text = "";
        int year;
        foreach (object o in eventList)
        {
            TextAsset t = (TextAsset)o;
            StringReader reader = new StringReader(t.text);
            name = reader.ReadLine();
            year = int.Parse(reader.ReadLine());
            text = reader.ReadToEnd();
            GameEvent e = new GameEvent(text, name, year);
            events.Add(year, e);
        }
    }
    


}
