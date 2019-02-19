using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    private GameObject player;
	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    protected void CheckForPlayer()
    {
        player = GameObject.Find("Player");
        Vector3 p = player.transform.position;
        p.y = transform.position.y;
        if (Vector3.Distance(transform.position, p) < 1)
        {
            Destroy(gameObject);
        }
    }
   
}
