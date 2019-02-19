using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProductionBuilding : Building {

    [SyncVar]
    public float WorkersGarrisoned;
    public float MaxWorkersGarrisoned;
    public int ResourceProduced; //0 = gold, 1 = mana
    public int ResourceQuantity;
    public GameObject WorkerPrefab;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(UpdateOwner());
	}

    private IEnumerator UpdateOwner()
    {
        yield return new WaitForSeconds(1);
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
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (health <= 0 && !isDie)
        {
            Die(1);
            isDie = true;
        }
    }


    public void GarrisonWorker(GameObject worker)
    {
        worker.GetComponent<Selectable>().Die(0.2f);
        WorkersGarrisoned++;
    }
    
}
