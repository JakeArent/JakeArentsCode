using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : Collectible {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckForPlayer();
	}

    private void OnDestroy()
    {
        GameObject.Find("Player").GetComponent<Player>().SetDyn();
    }
}
