using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GruntAI : Enemy {

    private NavMeshAgent NavAgent;
    private bool playerInRadius;
	// Use this for initialization
	void Start ()
    {
        NavAgent = transform.GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckForPlayer();
        checkState();
    }

    private void CheckForPlayer()
    {
        if (animController == null)
            animController = GetComponent<Animator>();
        if ((Vector3.Distance(transform.position, player.transform.position) < 2) && HP > 0)
        {
            animController.SetInteger("AnimState", 3);
            NavAgent.isStopped = true;
        }
        else if (Vector3.Distance(transform.position, player.transform.position) < DetectRadius && HP > 0)
        {
            animController.SetInteger("AnimState", 1);
            NavAgent.destination = player.transform.position;
            NavAgent.isStopped = false;
            playerInRadius = true;
        }
        else
        {
            playerInRadius = false;
            NavAgent.isStopped = true;
            animController.SetInteger("AnimState", 0);
        }
    }

    private void AttackPlayer ()
    {
        player.GetComponent<Player>().TakeDamage(DMG);
    }
}
