using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimTest : MonoBehaviour {

    public Animator ac;
    private int ani = 0;

	// Use this for initialization
	void Start () {
        StartCoroutine(SwitchAnim());
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public IEnumerator SwitchAnim()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Debug.Log("State:" + ani);
            ani++;
            if (ani > 6)
            {
                ani = 0;
            }
            ac.SetInteger("AnimState", ani);
            
        }
    }
}
