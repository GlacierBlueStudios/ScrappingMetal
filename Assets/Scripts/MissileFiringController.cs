using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileFiringController : MonoBehaviour
{
	public GameObject targetingSystem;

	private Transform[] missilePayload;

	private Transform[] targetPoints;


	private bool hasMissles = false;
	private bool isAlive = false;
	private int missileCount = 0;


	// Use this for initialization
	void Start () {
	
		missilePayload = new Transform[transform.childCount - 1];

		for (int i = 0; i < transform.childCount-1; i++) {
			missilePayload[i] = transform.GetChild (i).transform;	
		}

		missileCount = missilePayload.Length;

		targetPoints = targetingSystem.GetComponentsInChildren<Transform>();
	}
	
	// Update is called once per frame
	void Update () {

		if (missileCount > 0)
			hasMissles = true;
		else
			hasMissles = false;

		if (Input.GetKey (KeyCode.JoystickButton4)&& hasMissles) {

			targetingSystem.SetActive (true);
			if (Input.GetKey (KeyCode.JoystickButton5) && !isAlive) {
				FireMissile ();
			}
		} else 
		{
			targetingSystem.SetActive (false);
		}
	}

	int num = 0;
	private void FireMissile()
	{
		isAlive = true;

		Vector3[] points = new Vector3[targetPoints.Length];
		for (int i = 0; i < targetPoints.Length; i++) {
			points [i] = targetPoints [i].position;
		}
		missilePayload [num].GetComponent<MissileFlightController> ().FireMissile (points);
	
		num++;
	}


	public void LastMissileCallback()
	{
		isAlive = false;
	}
}
