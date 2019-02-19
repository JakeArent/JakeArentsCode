using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour {

    // Use this for initialization
    public GameObject player;
	void Start ()
    {
        Cursor.visible = true;
        Medium();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Easy()
    {
        GameObject inPlayer = Instantiate(player);
        inPlayer.transform.position = GameObject.Find("Player Spawn").transform.position;
        GameObject.Find("Player").GetComponent<Player>().HP = 20;
        FadeIn();
    }

    public void Medium()
    {
        GameObject.Find("Player").GetComponent<Player>().HP = 15;
        FadeIn();
    }

    public void Hard()
    {
        GameObject.Find("Player").GetComponent<Player>().HP = 10;
        FadeIn();
    }

    private void FadeIn()
    {
        Debug.Log("TEST");
        GameObject p = GameObject.Find("Panel").gameObject;
        p.SetActive(false);
    }
}
