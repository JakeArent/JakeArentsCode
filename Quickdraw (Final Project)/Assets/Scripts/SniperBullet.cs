using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBullet : MonoBehaviour
{


    private int damage;
    private Vector3 direction;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame 
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Rifle" || collision.gameObject.name == "Sniper")
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
