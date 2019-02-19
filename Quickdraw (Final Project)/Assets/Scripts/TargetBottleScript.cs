using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBottleScript : MonoBehaviour {

    public Level1Script l1Script;
	// Use this for initialization
	void Start ()
    {
        l1Script = GameObject.Find("Level1Floor").GetComponent<Level1Script>();
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

    private void OnDestroy()
    {
        l1Script.bottleCount--;
    }
}
