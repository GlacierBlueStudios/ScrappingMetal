using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : MonoBehaviour {

    public GameObject[] guns;

	private bool isShooting = false;
	private bool alreadyTriggered = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.JoystickButton0)) {
			isShooting = true;
		} else {
			isShooting = false;
		}
		
		if (isShooting && !alreadyTriggered) {
			for (int i = 0; i < guns [0].transform.childCount; i++) {
				guns [0].transform.GetChild (i).GetComponent<ParticleSystem> ().Play ();
				guns [1].transform.GetChild (i).GetComponent<ParticleSystem> ().Play ();
			}
			alreadyTriggered = true;
		} else if (!isShooting && alreadyTriggered)
		{
			for (int i = 0; i < guns [0].transform.childCount; i++) 
			{
				guns [0].transform.GetChild (i).GetComponent<ParticleSystem> ().Stop ();
				guns [1].transform.GetChild (i).GetComponent<ParticleSystem> ().Stop ();
			}
			alreadyTriggered = false;
		}
	}
}
