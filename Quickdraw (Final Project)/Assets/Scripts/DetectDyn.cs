using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectDyn : MonoBehaviour {

    private GameObject player;
    public AudioClip boom;

	// Use this for initialization
	void Start ()
    {
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Vector3.Distance(player.transform.position,transform.position) < 2 && player.GetComponent<Player>().GetDyn() == 2)
        {
            DestroyObstacles();
        }
	}

    private void DestroyObstacles()
    {
        AudioSource audio = GameObject.Find("Player").GetComponent<AudioSource>();
        audio.clip = boom;
        audio.Play();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(gameObject);
    }
}
