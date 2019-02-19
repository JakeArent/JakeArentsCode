using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {
    //private variables
    protected GameObject player;

    //public variables
    public float DetectRadius;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    protected void checkState()
    {
        if (animController == null)
            animController = GetComponent<Animator>();
        if (HP <= 0)
        {
            animController.SetInteger("AnimState", 2);
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
        Debug.Log("DED");
    }

}
