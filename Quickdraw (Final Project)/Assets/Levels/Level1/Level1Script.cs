using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Script : MonoBehaviour {
    [SerializeField]
    private GameObject bandit;
    private GameObject spawn1;
    private GameObject spawn2;
    private bool isSpawned;
    public float bottleCount;

    private List<GameObject> enemies;
    
	// Use this for initialization
	void Start () {
        spawn1 = GameObject.Find("EnemySpawn1");
        spawn2 = GameObject.Find("EnemySpawn2");
        bottleCount = 4;
        isSpawned = false;
        enemies = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if (bottleCount < 1 && !isSpawned)
        {
            GameObject e = Instantiate(bandit, spawn1.transform.position, Quaternion.identity);
            enemies.Add(e);
            e = Instantiate(bandit, spawn2.transform.position, Quaternion.identity);
            enemies.Add(e);
            isSpawned = true;
        }
        bool enemiesLeft = false;
        foreach (GameObject en in enemies)
        {
            if (en != null)
                enemiesLeft = true;
        }
        if (!enemiesLeft && isSpawned)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
	}
    


}
