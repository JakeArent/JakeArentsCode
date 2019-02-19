using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    //private variables
    private int dynCount; //This represents whether the player has retreived the Dynamite on level 2
    private bool canShoot = true;

    //public variables
    public float RefireTime = .5f;
    public GameObject bullet;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetAxisRaw("Fire1") > 0 && canShoot)
        {
            canShoot = false;
            Shoot();
            StartCoroutine(ShootReset());
        }
        GameObject.Find("HPText").GetComponent<UnityEngine.UI.Text>().text = HP.ToString();
    }

    private void Shoot () //Instantiate bullet and fire it
    {
        GameObject b = Instantiate(bullet);
        b.GetComponent<Bullet>().setDamage(DMG);
    }

    private IEnumerator ShootReset()
    {
        yield return new WaitForSeconds(RefireTime);
        canShoot = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //check for bullet impact
        if (collision.gameObject.GetComponent<SniperBullet>() != null && collision.transform.tag == "Bullet")
            TakeDamage(collision.gameObject.GetComponent<SniperBullet>().getDamage());

        Debug.Log(collision.gameObject.name);

        if (collision.gameObject.tag == "Collectible")
        {
            Debug.Log("HELLO");
            Destroy(collision.gameObject);
        }
    }

    public void HealPlayer(int healPower)
    {
        HP += healPower;
    }
    

    public void SetDyn()
    {
        dynCount++;
    }

    public int GetDyn()
    {
        return dynCount;
    }
}
