using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAI : Enemy {

    //Public variables
    public float RefireTime;
    public GameObject bullet;

    private bool canShoot = true;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.Find("Player");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (canShoot && HP > 0)
            ShootPlayer();
        checkState();
	}

    private void ShootPlayer()
    {
        //shoot is los is valid, then wait for the refire time;
        Debug.Log("Shoot");
        GameObject b = Instantiate(bullet);
        b.GetComponent<SniperBullet>().setDamage(DMG);
        b.transform.position = transform.position + transform.forward;
        b.transform.eulerAngles = new Vector3(90, 0, 0) + transform.eulerAngles;
        b.gameObject.GetComponent<Rigidbody>().velocity = (player.transform.position - transform.position) * .5f;
        canShoot = false;
        StartCoroutine(ReFire());
    }

    IEnumerator ReFire()
    {
        yield return new WaitForSeconds(RefireTime);
        canShoot = true;

    }
}
