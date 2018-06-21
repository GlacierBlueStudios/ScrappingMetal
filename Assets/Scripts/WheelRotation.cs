using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRotation : MonoBehaviour {
	public WheelCollider targetWheel;

	private Vector3 wheelPosition;
	private Quaternion wheelRotation;
	// Use this for initialization

	// Update is called once per frame
	void Update () {
		targetWheel.GetWorldPose (out wheelPosition, out wheelRotation);
		transform.position = wheelPosition;
		transform.rotation = wheelRotation;
	}
}
