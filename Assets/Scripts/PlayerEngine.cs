using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEngine : MonoBehaviour {

	public Rigidbody rb;

	public WheelCollider lfWheel;
	public WheelCollider rfWheel;
	public WheelCollider lrWheel;
	public WheelCollider rrWheel;

	public float maxSpeed;
	public float maxTurnSpeed;
	public float maxSteeringAngle;
	public float maxBrakeTorque;
	public float maxTourqe;

	public float currentAccel;
	public float currentBrake;
	public float currentSteering;
	public float currentSpeed;


	public float targetSteeringAngle;
	public float accelSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetInput ();
		Drive ();

		lfWheel.steerAngle = Input.GetAxis ("Horizontal") * maxSteeringAngle;
		rfWheel.steerAngle = Input.GetAxis ("Horizontal") * maxSteeringAngle;
	}


	private void Drive()
	{
		if (currentSpeed < maxSpeed && currentAccel > 0) {
			rrWheel.motorTorque += maxTourqe * Time.deltaTime*accelSpeed*currentAccel;
			lrWheel.motorTorque += maxTourqe * Time.deltaTime*accelSpeed*currentAccel;

		} else if (lrWheel.motorTorque > 0 || rrWheel.motorTorque > 0) {
			rrWheel.motorTorque -= maxTourqe ;
			lrWheel.motorTorque -= maxTourqe ;

		} else {
			rrWheel.motorTorque = 0;
			lrWheel.motorTorque = 0;
		}

		if (Input.GetAxis("Brake")>0) {
			lfWheel.brakeTorque += maxBrakeTorque * currentBrake;
			rfWheel.brakeTorque += maxBrakeTorque * currentBrake;
		} else {
			lfWheel.brakeTorque = 0;
			rfWheel.brakeTorque = 0;
		}

		Debug.Log (rrWheel.motorTorque + " " + lrWheel.motorTorque);
		currentSpeed = rb.velocity.magnitude*2;

	}

	private void GetInput()
	{
		if (Input.GetAxis ("Accel")>0) {
			if (currentAccel < 1)
				currentAccel += Time.deltaTime * accelSpeed;
			else
				currentAccel = 1;
		} else 
			currentAccel = 0;

		if (Input.GetAxis("Brake")>0) {
			if (currentBrake < 1)
				currentBrake += Time.deltaTime * accelSpeed;
			else
				currentBrake = 1;
		} else
			currentBrake = 0;


		currentSteering = Input.GetAxis ("Horizontal");
	}
}
