using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spearmen : Unit {

    public bool hasUpgrade;

	// Use this for initialization
	void Start () {
        StartCoroutine(SlowUp());
        DetectionRange = 5;
        AttackRange = 1.5f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
