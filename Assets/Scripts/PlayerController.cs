using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[Header("Car Parts")]
	public Transform carBody;
	public Transform lfWheel;
	public Transform rfWheel;
	public Transform rrWheel;
	public Transform lrWheel;

	[Header("Car Settings")]
	public float frontWheelRadius;  
	public float rearWheelRadius;
	public float tireTraction;
	public float dragFromAir;
	public float dragFromTerrain;         	// this drag is to add ground effects to the speed ie sand, water, oil etc
	public float brakeForce;

	[Header("Max Values")]
	public float maxTopSpeed;
	public float maxAccelTime;              // how quick will we get up to speed
	public float maxSteerAngle;
	public float maxTurnSpeed;
	public float maxOverSteer; 				// maxoversteer allows us to slide the car visually
	public float maxBrakeTorque;
	[Header("Current Values")]
	public float currentSpeed;
	public float currentThrust;				// stores thrust from controls
	public float adjustedThrust;  			// used to store any thrust difference past controls
	public float finalThrust; 				// final thrust is and added effects ie boost, damage etc
	public float currentSteering;
	public float currentOverSteerVelo;
	public float currentBrakeTorque;

	public bool  currentlyGrounded;
	public bool  currentlyOffroad;
	public bool  currentlyUpsideDown;

	// private variables
	private WheelCollider[] wheelColliders;

	private GameObject steeringFL;
	private GameObject steeringFR;

	private Transform visualBody;

	private Rigidbody rb;

	private const float m2m = 1/1609.344f;
	private const float s2h = 3600;

	#region Unity Functions
	// Use this for initialization
	void Start () {
		CreateVisualBody ();

		wheelColliders = new WheelCollider[4];
		wheelColliders[0] = CreateWheelCollider(lfWheel.position, frontWheelRadius);
		wheelColliders[1] = CreateWheelCollider(rfWheel.position, frontWheelRadius);
		wheelColliders[2] = CreateWheelCollider(lrWheel.position, rearWheelRadius);
		wheelColliders[3] = CreateWheelCollider(rrWheel.position, rearWheelRadius);

		// create some extra transforms so we can rotate the front wheels more easily for steering
		steeringFL = new GameObject("SteeringFL");
		steeringFR = new GameObject("SteeringFR");
		steeringFL.transform.position = lfWheel.position;
		steeringFR.transform.position = rfWheel.position;
		steeringFL.transform.rotation = lfWheel.rotation;
		steeringFR.transform.rotation = rfWheel.rotation;
		steeringFL.transform.parent = lfWheel.parent;
		steeringFR.transform.parent = rfWheel.parent;
		lfWheel.parent = steeringFL.transform;
		rfWheel.parent = steeringFR.transform;


		rb = GetComponent<Rigidbody>();

		// set an artificially low center of gravity to aid in stability.
		Vector3 frontAxleCenter = 0.5f * (lfWheel.localPosition + rfWheel.localPosition);
		Vector3 rearAxleCenter = 0.5f * (lrWheel.localPosition + rrWheel.localPosition);
		Vector3 vehicleCenter = 0.5f * (frontAxleCenter + rearAxleCenter);
		float avgWheelRadius = 0.5f * (frontWheelRadius + rearWheelRadius);
		rb.centerOfMass = vehicleCenter - 0.8f*avgWheelRadius*Vector3.up;
	}

	// Update is called once per frame
	void FixedUpdate () {
		currentThrust = Input.GetAxis ("Accel");
		currentSteering = Input.GetAxis("Horizontal");

		if (currentThrust <= 0 && currentSpeed > 0) 
		{
			brakeForce = Mathf.Abs (currentThrust);
			currentThrust = 0;

		}
		// calculate our current velocity in local space (i.e. so z is forward, x is sideways etc)
		Vector3 relVel = transform.InverseTransformDirection(rb.velocity);

		Debug.Log (rb.velocity);
		// our current speed is the forward part of the velocity - note this will be negative if we are reversing.
		currentSpeed = relVel.z * m2m * s2h;
	
		finalThrust = currentThrust;

		// cast a ray to check if we are grounded or not
		Vector3 frontWheelBottom = 0.5f*(rfWheel.position + lfWheel.position) - new Vector3(0,0.5f,1.0f) * frontWheelRadius;
		RaycastHit hit;
		currentlyGrounded = Physics.Raycast(frontWheelBottom, -Vector3.up, out hit, 2.0f*frontWheelRadius);

		// check if the ground beneath us is tagged as 'off road'
		if(currentlyGrounded)
			currentlyOffroad = hit.collider.gameObject.CompareTag("OffRoad");

		// check if the vehicle has overturned. we don't do anything with this, but a controller script could use it
		// to reset a vehicle that has been overturned for a certain amount of time for example.
		currentlyUpsideDown = transform.up.y < 0.0f;

		// only apply thrust if the wheels are touching the ground
		if(currentlyGrounded)
			ApplyThrust();

		ApplyDrag();
		ApplySteering();

		RotateWheels (relVel);
	}
	#endregion

	#region VisualFunctions
	private void RotateWheels(Vector3 currentVelo)
	{
		float wheelRotationFront = (currentVelo.z / frontWheelRadius) * Time.deltaTime * Mathf.Rad2Deg;
		float wheelRotationRear = (currentVelo.z / rearWheelRadius) * Time.deltaTime * Mathf.Rad2Deg;
		// now rotate each wheel
		lfWheel.Rotate(wheelRotationFront, 0.0f, 0.0f);
		rfWheel.Rotate(wheelRotationFront, 0.0f, 0.0f);
		lrWheel.Rotate(wheelRotationRear, 0.0f, 0.0f);
		rrWheel.Rotate(wheelRotationRear, 0.0f, 0.0f);
	}
	#endregion
	#region Physics Functions
	private void ApplyDrag()
	{
		if (currentSpeed != 0) {
			// get our velocity relative to our local orientation (i.e. forward motion is along z-axis etc)
			Vector3 relVel = transform.InverseTransformDirection (rb.velocity);

			Vector3 drag = Vector3.zero;

			// calculate our drag coeeficients based on the current handling parameters

			// strength of drag force resisting the vehicle's forward motion
			float forwardDrag = Mathf.Lerp (0.1f, 0.5f, tireTraction);
			// strength of drag force resisting the vehicle's sideways motion
			float lateralDrag = Mathf.Lerp (1.0f, 5.0f, tireTraction);
			// strength of drag that slows the vehicle down when thrust is not pressed (basically just affects deceleration time)
			float engineDrag = Mathf.Lerp (0.0f, 5.0f, brakeForce*2);

			// calculate drag in forward direction
			// engine drag slows the vehicle down when the accelerator is not being pressed
			drag.z = relVel.z * (forwardDrag + ((1.0f - Mathf.Abs (currentThrust)) * engineDrag));
			// add some additional drag when driving off road
			if (currentlyOffroad)
				drag.z += relVel.z * dragFromTerrain;

			// lateral (sideways drag) slows the vehicle in the direction perpendicular to that in which it is facing.
			drag.x = relVel.x * lateralDrag/2;

			// when the vehicle is not grounded, reduce the drag force.
			if (!currentlyGrounded)
				drag *= dragFromAir;

			// transform the drag force back into world space
			drag = transform.TransformDirection (drag);

			// apply the drag by reducing our current velocity directly
			Vector3 vel = rb.velocity;
			vel -= drag * Time.deltaTime;
			rb.velocity = vel;
		}
	}


	private void ApplySteering()
	{
		float steerAngle = currentSteering * maxSteerAngle;

		// rotate the front wheels
		steeringFL.transform.localRotation = Quaternion.Euler(0, steerAngle, 0);
		steeringFR.transform.localRotation = Quaternion.Euler(0, steerAngle, 0);

		// only turn the vehicle when we're on the ground and moving
		if(currentlyGrounded && rb.velocity.sqrMagnitude > 1f)
		{
			// reverse the steering direction when the vehicle is moving backwards
			Vector3 relVel = transform.InverseTransformDirection(rb.velocity);
			steerAngle *= Mathf.Sign(relVel.z);

			// rotate the vehicle
			Quaternion steerRot = Quaternion.Euler(0, steerAngle * Time.deltaTime * (1.0f + 2.0f*maxTurnSpeed), 0);
			rb.MoveRotation(transform.rotation * steerRot);

		}
	}

	private void ApplyThrust()
	{
		float accelerationTime = 0;

		accelerationTime = Mathf.Max(0.01f, accelerationTime);

		float topSpeedMetresPerSec = maxTopSpeed / (m2m * s2h);
		// limit the speed the vehicle can move at in reverse
		float topSpeedReverse = 0.2f * maxTopSpeed;
		// calculate our acceleration value in m/s^2
		float accel = topSpeedMetresPerSec / maxAccelTime;

		// if we're at or over the top speed, then don't accelerate any more
		if(currentSpeed >= maxTopSpeed || currentSpeed <= -topSpeedReverse)
			accel = 0.0f;

		// calculate our final acceleration vector
		Vector3 thrustDir = transform.forward;
		Vector3 accelVec = accel * thrustDir * currentThrust;

		// add our acceleration to our current velocity
		Vector3 vel = new Vector3(rb.velocity.x,0,rb.velocity.z);
		vel += accelVec * Time.deltaTime;
		rb.velocity = vel;


		currentBrakeTorque = 0;
		// apply the brakes automatically when the throttle is off to stop the vehicle rolling by itself.
		// modify the braking amount based on the current speed so we come to a gentle stop.
		if((currentThrust > -.01f && currentThrust < .01f) && currentSpeed < 10.0f)
			currentBrakeTorque = maxBrakeTorque * Mathf.Clamp01(brakeForce * (10.0f - currentSpeed));
		foreach (WheelCollider wheel in wheelColliders) {
			wheel.brakeTorque = currentBrakeTorque;
		}
	}

	#endregion

	#region Creation Functions
	private WheelCollider CreateWheelCollider(Vector3 position, float radius)
	{
		GameObject wheel = new GameObject("WheelCollision");
		wheel.transform.parent = carBody;
		wheel.transform.position = position;
		wheel.transform.localRotation = Quaternion.identity;

		WheelCollider collider = wheel.AddComponent<WheelCollider>();
		collider.radius = radius;
		collider.suspensionDistance = 0.1f;
	
		WheelFrictionCurve sideFriction = collider.sidewaysFriction;
		sideFriction.stiffness = 0.01f;
		collider.sidewaysFriction = sideFriction;

		return collider;
	}
	private void CreateVisualBody()
	{
		visualBody = new GameObject ("VisualBody").transform;
		visualBody.position = carBody.position;
		visualBody.rotation = carBody.rotation;
		visualBody.localScale = Vector3.one;

		while (carBody.childCount > 0)
			carBody.GetChild (0).parent = visualBody;

		visualBody.parent = carBody;
	}
	#endregion
}
