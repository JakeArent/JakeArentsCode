using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    //public variables
    public int HP; // the health of the character
    public float Speed; //how fast the chartacter moves. Sent to the NavMeshAgent on AI
    public int DMG; //How much damage this character will deal on a successful attack

    //private variables
    protected Animator animController; //The animation controller attatched to the character

	// Use this for initialization
	void Start ()
    {
        animController = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void TakeDamage(int damageTaken)
    {
        HP -= damageTaken;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (collision.gameObject.GetComponent<SniperBullet>() != null)
                TakeDamage(collision.gameObject.GetComponent<SniperBullet>().getDamage());
            if (collision.gameObject.GetComponent<Bullet>() != null)
                TakeDamage(collision.gameObject.GetComponent<Bullet>().getDamage());
        }
    }
}
