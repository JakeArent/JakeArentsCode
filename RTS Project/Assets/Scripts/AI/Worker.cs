using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Worker : Unit {

    // Use this for initialization
    public List<GameObject> BuildableBuildings = new List<GameObject>();

    private GameObject toGarrison = null;
    private GameObject toBuild = null;

    private bool isBuilding = false;

	void Start () {
        StartCoroutine(SlowUp());
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (State != "Build" && isBuilding)
        {
            isBuilding = false;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                if (p.GetComponent<PlayerController>().Team == Team)
                    p.GetComponent<PlayerController>().Gold += toBuild.GetComponent<Building>().Cost;
            }
            
            Destroy(toBuild);
            toBuild = null;
        }
	}

    public void Build(GameObject building)
    {
        //build building
        toBuild = building;
        MovementWaypoint = toBuild.transform.position;
        agent.destination = MovementWaypoint;
        isBuilding = true;
        State = "Build";
    }

    public void Garrison(GameObject b)
    {
        //get in there!
        toGarrison = b;
        agent.destination = MovementWaypoint;
        
        State = "Garrison";
    }

    override
    protected void DetermineAction()
    {
        switch (State)
        {
            default:
                if (isServer)
                {
                    GetComponent<Animator>().SetInteger("AnimState", 0);
                    Rpc_SetAnimState(0);
                }
                break;

            case "Moving":
                if (isServer)
                {
                    GetComponent<Animator>().SetInteger("AnimState", 2);
                    Rpc_SetAnimState(2);
                }
                agent.isStopped = false;
                break;

            case "Attacking":
                agent.isStopped = true;
                if (Target != null)
                    transform.LookAt(Target.transform.position);
                Attack(Target);
                break;

            case "Attack Moving":
                if (Target == null)
                {
                    agent.destination = MovementWaypoint;
                    agent.isStopped = false;
                }
                else
                {
                    agent.isStopped = true;
                    transform.LookAt(Target.transform);
                    Attack(Target);
                }
                break;

            case "Death":
                break;

            case "Garrison":
                agent.isStopped = false;
                if (isServer)
                {
                    GetComponent<Animator>().SetInteger("AnimState", 2);
                    Rpc_SetAnimState(2);
                }

                if (agent.remainingDistance < 1)
                {
                    //garrison worker
                    StopAllCoroutines();
                    toGarrison.GetComponent<ProductionBuilding>().GarrisonWorker(gameObject);
                }
                break;

            case "Build":
                agent.isStopped = false;
                if (isServer)
                {
                    GetComponent<Animator>().SetInteger("AnimState", 2);
                    Rpc_SetAnimState(2);
                }
                if (agent.remainingDistance < 1.5)
                {
                    //build
                    Vector3 pos = toBuild.transform.position;
                    
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject p in players)
                    {
                        if (p.GetComponent<PlayerController>().Team == Team)
                        {
                            p.GetComponent<PlayerController>().Cmd_SpawnBuilding(toBuild.GetComponent<Selectable>().Name,pos, gameObject);
                        }
                    }
                    Destroy(toBuild);
                    State = "Idle";
                    isBuilding = false;
                }
                break;
        }
    }
    
    override
    protected string UpdateState()
    {
        if (health <= 0 && !isDying)
        {
            isDying = true;
            Die(3);
            return "Death";
        }
        else if (isDying)
            return "Death";
        else if (State == "Garrison" || State == "Build")
            return State;
        else if (State == "Moving" && !agent.isStopped)
            return State;
        else if ((State == "Attacking" && Target != null) || (Target != null && State == "Idle"))
            return "Attacking";
        else
            return "Idle";
    }
}
