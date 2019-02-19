using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {


    private int damage;
    private float speed = 3f;
    private Vector3 direction;
	// Use this for initialization
	void Start ()
    {
        
        GameObject pistol = GameObject.Find("Pistol");
        transform.position = pistol.transform.position;
        transform.eulerAngles = new Vector3(90, 0, 0) + pistol.transform.eulerAngles;
        gameObject.GetComponent<Rigidbody>().velocity = pistol.transform.forward * speed;

    }
	
	// Update is called once per frame 
	void Update ()
    {
        
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Pistol" || collision.gameObject.name == "Player")
        {
            
        }
        else
        { 
            Destroy(gameObject);
        }
    }

    public void setDamage(int dmg)
    {
        damage = dmg;
    }

    public int getDamage()
    {
        return damage;
    }
}
