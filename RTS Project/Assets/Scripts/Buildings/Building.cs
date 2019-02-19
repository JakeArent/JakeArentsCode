using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Building : Selectable
{
    public List<GameObject> ProductionQueue = new List<GameObject>();

    public float ProductionTime;
    [SyncVar]
    public float ProductionProgress;
    public int EnergyConsumption;
    public GameObject SpawnPoint;
    public Vector3 RallyPoint;
    public List<GameObject> BuildableUnits = new List<GameObject>();
   

    public GameObject Owner = null;

    protected bool isDie = false;
    

    // Use this for initialization
    void Start()
    {
        StartCoroutine(ProgressIncrease());
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && !isDie)
        {
            Die(1);
            Debug.Log("IsDying");
            isDie = true;
            if (this.gameObject.tag == "Base")
            {
                Debug.Log("Got Right Buildig");
                StartCoroutine(victoryScreen());
            }

        }
    }

    public void CreateUnit()
    {

    }

    public virtual void QueueUnit(GameObject UnitType)
    {
        ProductionQueue.Add(UnitType);
    }

    public void SetRallyPoint()
    {

    }

    public IEnumerator ProgressIncrease()
    {
        while (true)
        {
            if (Owner == null)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject p in players)
                {
                    if (p.GetComponent<PlayerController>().Team == Team)
                    {
                        Owner = p;
                    }
                }
            }
            yield return new WaitForSeconds(1);
            //getEnergyPerSecond
            if (ProductionQueue.Count > 0)
            {
                float eff = 0;
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject p in players)
                {
                    if (p.GetComponent<PlayerController>().Team == Team)
                        eff = p.GetComponent<PlayerController>().Efficiency;

                }
                ProductionProgress += (1 *eff/100); //(1 * efficiency)
            }
            if (ProductionProgress >= ProductionTime)
            {
                Debug.Log("Production Finished");
                if (!isServer)
                {
                    Owner.GetComponent<PlayerController>().Cmd_SpawnUnit(gameObject);

                    ProductionQueue.RemoveAt(0);
                    ProductionProgress = 0;
                }
            }
        }
    }

    public void SpawnUnit()
    {
        //instantiate
        GameObject u = Instantiate(ProductionQueue[0], SpawnPoint.transform.position, Quaternion.identity);
        //set team
        u.GetComponent<Selectable>().Team = Team;
        //network spawn
        NetworkServer.Spawn(u);
        //remove the object from the queue for server
        ProductionQueue.RemoveAt(0);
        ProductionProgress = 0;
    }

    public IEnumerator victoryScreen()
    {
        Debug.Log("CRStarted");
        yield return new WaitForSeconds(1);
        if(Team == 1)
        {
            SceneManager.LoadScene("Team2Victory");
        }
        else if (Team == 2)
        {
            SceneManager.LoadScene("Team1Victory");
        }
    }
}
