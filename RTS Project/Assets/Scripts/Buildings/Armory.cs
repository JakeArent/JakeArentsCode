using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armory : Building {

    List<Upgrade> AvailableUpgrades;
    List<Upgrade> UpgradeQueue;
    float Progress = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CompleteUprade()
    {

    }

    public void QueueUpgrade(Upgrade uq)
    {

    }

    public IEnumerator UpgradeProgress()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Progress += 1; // * (getManaPS)
        }
    }
}
