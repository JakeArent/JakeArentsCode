using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Manager : MonoBehaviour {

    // Use this for initialization
    public GameObject[] enemies;
	void Start ()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
	}
	
	// Update is called once per frame
	void Update ()
    {
        bool enemiesLeft = false;
        foreach (GameObject en in enemies)
        {
            if (en != null)
                enemiesLeft = true;
        }
        if (!enemiesLeft)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(3);
        }
    }
}
