using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class BossAI : Enemy {

    NavMeshAgent myNav = null;
    private GameObject bossPistol;
    private bool canFire;
    private float fireRate;
    private bool canSpin;
    private float spinRate;
    private float beginAttack = 20; //Distance that tells boss to attack once player enters the boss arena
    public GameObject bullet;

    // Use this for initialization
    void Start () {
        bossPistol = GameObject.Find("BossPistol");
        player = GameObject.Find("Player");
        fireRate = 2;
        spinRate = 10;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.LookAt(player.transform.position);
        if (canSpin)
        {
            canSpin = false;
            SpinAttack();
            StartCoroutine(SpinAttackRate());
            fireRate = 2;
        }

        if (canFire)
        {
            Shoot();
            StartCoroutine(ShootReset());
        }
	}

    private void fightPlayer()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) < beginAttack)
        {
            myNav.destination = player.transform.position;
            myNav.isStopped = false;
        }
        //if player enters arena begin attack
    }

    private void Shoot() //Instantiate bullet and fire it
    {
        GameObject b = Instantiate(bullet);
        b.GetComponent<Bullet>().setDamage(DMG);
    }

    private IEnumerator SpinAttackRate()
    {
        yield return new WaitForSeconds(spinRate);
        canSpin = true;
    }

    void SpinAttack()
    {
        fireRate = 1/2;
        this.transform.RotateAround(this.transform.position, Vector3.up, 10 * Time.deltaTime);
    }

    private IEnumerator ShootReset()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}
