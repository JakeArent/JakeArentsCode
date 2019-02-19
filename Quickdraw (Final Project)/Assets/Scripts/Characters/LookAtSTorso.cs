using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtSTorso : MonoBehaviour {

    private GameObject player;

    // Use this for initialization
    void Start () {
        player = GameObject.Find("Player");
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 p = player.transform.position;
        p.y = transform.position.y;
        this.transform.LookAt(player.transform.position);
    }
}
