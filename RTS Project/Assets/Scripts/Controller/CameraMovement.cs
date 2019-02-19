using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    private int screenH;
    private int screenW;

	void Start () {

        screenH = Screen.height;
        screenW = Screen.width;

        StartCoroutine(slowUpdate());

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public IEnumerator slowUpdate()
    {
        while (true)
        {
            try
            {
                if(Input.mousePosition.x >= screenW - 10)
                {
                    Camera.main.transform.position += new Vector3(0.5f, 0, 0.5f);
                }

                if (Input.mousePosition.x <= 0 + 10)
                {
                    Camera.main.transform.position -= new Vector3(0.5f, 0, 0.5f);
                }

                if (Input.mousePosition.y >= screenH - 10)
                {
                    Camera.main.transform.position += new Vector3(-0.5f, 0, 0.5f);
                }

                if (Input.mousePosition.y <= 0 + 10)
                {
                    Camera.main.transform.position -= new Vector3(-0.5f, 0, 0.5f);
                }

            }
            catch
            {

            }

            yield return new WaitForSeconds(.05f);
        }
    }
}
