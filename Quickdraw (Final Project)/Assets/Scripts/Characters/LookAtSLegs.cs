using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtSLegs : MonoBehaviour {

    private GameObject player;
    private Vector3 playerPosition;
    private Vector3 legOffset;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        playerPosition = player.transform.position;
        legOffset = this.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        playerPosition = player.transform.position;
        legOffset = this.transform.position;
        playerPosition.y = legOffset.y;
        this.transform.LookAt(playerPosition);
    }
}
