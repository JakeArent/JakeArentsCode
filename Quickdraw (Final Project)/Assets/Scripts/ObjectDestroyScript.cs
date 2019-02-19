using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyScript : MonoBehaviour {

    private GameObject bullet;

	// Use this for initialization
	void Start () {
        bullet = GameObject.Find("Bullet");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Destroy(this.gameObject);
        }
    }
}
