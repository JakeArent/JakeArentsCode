using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Manager : MonoBehaviour {

    public List<GameObject> enemies;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        bool enemiesLeft = false;
        foreach (GameObject en in enemies)
        {
            if (en != null)
                enemiesLeft = true;
        }
        if (!enemiesLeft)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }

    }
}
