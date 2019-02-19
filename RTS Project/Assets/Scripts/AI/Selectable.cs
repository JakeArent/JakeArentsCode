using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Selectable : NetworkBehaviour {
    [SyncVar]
    public int health;

    public int MaxHealth;
    public string Name;
    public int Cost;
    [SyncVar]
    public int Team;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(int DmgAmount)
    {
        health -= DmgAmount;
    }

    public void Die(float time)
    {
        StopAllCoroutines();
        int deathAnimNum = Random.Range(3, 5);
        if (GetComponent<Animator>() != null)
            GetComponent<Animator>().SetInteger("AnimState", deathAnimNum);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<PlayerController>().Team == Team)
                p.GetComponent<PlayerController>().DelUnit(gameObject);
        }
        StartCoroutine(despawnTimer(time));
    }

    public void Spawn()
    {
        //instantiate
    }

    public IEnumerator despawnTimer(float time)
    {
        yield return new WaitForSeconds(time);
        NetworkServer.Destroy(this.gameObject);
    }
}
