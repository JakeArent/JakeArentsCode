using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class Swordsman : Unit {

    public bool hasUpgrade;

    // Use this for initialization
    void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(SlowUp());
        DetectionRange = 5;
        AttackRange = 1;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    
}
