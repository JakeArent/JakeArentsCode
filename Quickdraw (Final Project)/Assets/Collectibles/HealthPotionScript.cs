using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionScript : Collectible {

    public int HealPower = 5;

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
        GameObject.Find("Player").GetComponent<Player>().HealPlayer(HealPower);
    }
}
