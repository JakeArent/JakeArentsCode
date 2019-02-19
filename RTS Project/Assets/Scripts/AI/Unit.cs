using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class Unit : Selectable {

    public int Damage;
    protected float Speed;
    public float AttackSpeed;

    protected float AttackRange;
    protected float DetectionRange;

    public string State = "Idle";

    [SyncVar]
    public GameObject Target;

    protected bool isDying = false;
    protected bool canAttack = true;

    public Vector3 MovementWaypoint = new Vector3();

    public NavMeshAgent agent;

    protected bool aMove = false;
    


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //set animations based on movement

    }

    public IEnumerator SlowUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            //targetting
            if (Target == null)
                GetNearestTarget();

            if (Target != null)
                if ((Target.GetComponent<Selectable>().health <= 0) || Vector3.Distance(transform.position, Target.transform.position) > DetectionRange)
                    Target = null;

            //act according to state
            DetermineAction();
            if (agent.remainingDistance < .2)
            {
                agent.isStopped = true;
            }
            State = UpdateState();
        }
        
    }

    protected virtual void DetermineAction()
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
                {
                    transform.LookAt(Target.transform.position);
                    Attack(Target);
                }
                break;

            case "Attack Moving":
                if (Target == null)
                {
                    agent.destination = MovementWaypoint;
                    if (isServer)
                    {
                        GetComponent<Animator>().SetInteger("AnimState", 2);
                        Rpc_SetAnimState(2);
                    }
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
        }
    }

    protected virtual string UpdateState()
    {
        if (health <= 0 && !isDying)
        {
            isDying = true;
            Die(3);
            return "Death";
        }
        else if (isDying)
            return "Death";
        else if ((State == "Moving" || State == "Attack Moving") && !agent.isStopped)
            return State;
        else if ((State == "Attacking" && Target != null) || (Target != null && State == "Idle"))
            return "Attacking";
        else
            return "Idle";
    }

    public void Move(string MoveType)
    {
        if (MoveType == "Attack Move")
        {
            aMove = true;
            State = "Attack Moving";
        }
        else
        {
            aMove = false;
            State = "Moving";
        }
        agent.destination = MovementWaypoint;
        agent.isStopped = false;
    }

    public void Attack(GameObject target)
    {
        float atkrng = AttackRange;
        //check range
        if (Target.GetComponent<Building>() != null)
            atkrng += 2;
        //if out of range, move closer
        if (Vector3.Distance(transform.position,Target.transform.position) > atkrng)
        {
            agent.destination = Target.transform.position;

            if (isServer)
            {
                GetComponent<Animator>().SetInteger("AnimState", 2);
                Rpc_SetAnimState(2);
            }
            agent.isStopped = false;
        }
        //if in range, attack;
        else if (canAttack)
        {
            canAttack = false;


            if (isServer)
            {
                GetComponent<Animator>().SetInteger("AnimState", 1);
                Rpc_SetAnimState(1);
            }
            target.GetComponent<Selectable>().TakeDamage(Damage);
            StartCoroutine(AtkCool());
        }
    }

    public IEnumerator AtkCool()
    {
        yield return new WaitForSeconds(AttackSpeed);
        canAttack = true;
    }

    public Selectable GetTarget()
    {
        //get target selectable
        Selectable h = new Selectable();
        return h;
    }

    public void GetNearestTarget()
    {
        //get nearest selectable
        List<GameObject> nearestTeam = new List<GameObject>();

        GameObject refPlayer = GameObject.FindGameObjectWithTag("Player");
        
        

        if (Team == 1)
        {
            foreach (GameObject u in refPlayer.GetComponent<PlayerController>().Team2Units)
            {
                nearestTeam.Add(u);
            }
            foreach (GameObject u in refPlayer.GetComponent<PlayerController>().Team2Buildings)
            {
                nearestTeam.Add(u);
            }
        }
        else if (Team == 2)
        {
            foreach (GameObject u in refPlayer.GetComponent<PlayerController>().Team1Units)
            {
                nearestTeam.Add(u);
            }
            foreach (GameObject u in refPlayer.GetComponent<PlayerController>().Team1Buildings)
            {
                nearestTeam.Add(u);
            }
        }
        
        
        foreach (GameObject g in nearestTeam)
        {
            if ( g != null && Vector3.Distance(g.transform.position, this.transform.position) < DetectionRange)
            {
                Target = g;
            }
        }
    }

    [ClientRpc]
    public void Rpc_SetAnimState(int state)
    {
        GetComponent<Animator>().SetInteger("AnimState", state);
    }
}
