using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AIControllerTrack : MonoBehaviour {

	[Header("Waypoints")]
	public List<Transform> nodes;
	public Transform path;
	public Vector3 nexPos;
	public int currentNode = 1;

	[Header("Sensors")]
	public Vector3 frontSensorPos = new Vector3 (0f, .25f, .4f);
	public float frontSideSensorPos = .3f;
	public float frontSensorAngle = 30;
	public float sensorLength = 3f;


	[Header("Rigidbody")]
	public Rigidbody rigid;

	[Header("Car Maximums")]
	public float maxSteerAngle = 45f;
	public float maxSpeed = 100f;
	public float maxMotorTorque = 80f;
	public float maxBrakeTorque = 150;
	public float maxTurnSpeed = 5;
	public float maxStuckTimer = 3;
	[Header("Wheels")]
	public WheelCollider wheelFL;
	public WheelCollider wheelFR;
	public WheelCollider wheelRL;
	public WheelCollider wheelRR;

	[Header("Current Logic")]
	public float currentSpeed;
	public float targetSteerAngle;
	public float driveDirection = 1;
	public float avoidMultiplier;
	public float brakingMultiplier;
	public float stuckTimer;
	public bool isReversing;
	public bool isBraking;
	public bool isSteering;
	public bool isAvoiding;
	public bool isFrontAvoiding;
	public bool debugCar;
	public bool isStuck;
	public bool hasCheckedPosition;
	public int currentLapCount;
	public int currentPosition;



	private bool startedMoving;
	private void Start()
	{
		
		Transform[] pathTransforms = path.GetComponentsInChildren<Transform> ();
		nodes = new List<Transform> ();

		for (int i = 0; i < pathTransforms.Length; i++) 
		{
			if (pathTransforms [i] != path.transform) 
			{
				nodes.Add (pathTransforms [i]);
			}
		}

		rigid = GetComponent<Rigidbody> ();
		rigid.centerOfMass = new Vector3 (0, .2f, 0);
		currentLapCount = 0;
		nexPos = new Vector3 (Random.Range (nodes [currentNode].position.x - 4, nodes [currentNode].position.x + 4), transform.position.y, Random.Range (nodes [currentNode].position.z - 4, nodes [currentNode].position.z + 4));
		//nexPos = new Vector3(Random.Range(0,200), 1,Random.Range(-60,160));
	}


	private void FixedUpdate()
	{
		CheckWaypointDistance ();
		Sensors ();
		ApplySteer ();
		SmoothSteer ();
		Braking ();
		Drive ();

	
	}


	private void ApplySteer()
	{		
		Vector3 relativeVector = transform.InverseTransformPoint (nexPos);
		float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
		targetSteerAngle = newSteer;
	}

	private void Braking()
	{
		if ((isAvoiding && currentSpeed > maxTurnSpeed*1.5f) || targetSteerAngle > maxSteerAngle*.75f) {
			isBraking = true;
			brakingMultiplier = Mathf.Abs (avoidMultiplier) * currentSpeed / maxSpeed;
		} else {
			isBraking = false;
			brakingMultiplier = 0;
		}

		if (isBraking ) {
			wheelRR.brakeTorque += maxBrakeTorque * Mathf.Abs(brakingMultiplier);
			wheelRL.brakeTorque += maxBrakeTorque * Mathf.Abs(brakingMultiplier);
		} else {
			wheelRR.brakeTorque = 0;
			wheelRL.brakeTorque = 0;

			if (isFrontAvoiding && currentSpeed < 1 && startedMoving) {
				driveDirection = -20;
			} else
				driveDirection = 1;
		}
	}
		
	private void CheckWaypointDistance()
	{
		if (Vector3.Distance (transform.position, nexPos) < 10) {

			if (currentNode == nodes.Count - 1) {
				currentLapCount++;
				currentNode = 0;
			} else {
				currentNode++;
			}
			if (isSteering)
				nexPos = new Vector3 (Random.Range (nodes [currentNode].position.x - .5f, nodes [currentNode].position.x + .5f), transform.position.y, Random.Range (nodes [currentNode].position.z - .5f, nodes [currentNode].position.z + .5f));
			else
				nexPos = new Vector3 (Random.Range (nodes [currentNode].position.x - 2, nodes [currentNode].position.x + 2), transform.position.y, Random.Range (nodes [currentNode].position.z - 2, nodes [currentNode].position.z + 2));

			startedMoving = true;
			if (!hasCheckedPosition) {
				hasCheckedPosition = true;
				currentPosition = nodes [currentNode].gameObject.GetComponent<WaypointController> ().WaypointCheckpoint (currentLapCount, currentPosition);
			}
		//	nexPos = new Vector3(Random.Range(0,200), 1,Random.Range(-60,160));
		} 
		else 
		{
			hasCheckedPosition = false;
		}


	}

	private void Drive()
	{
		currentSpeed = Mathf.Abs (rigid.velocity.magnitude * Mathf.PI); 


		if (currentSpeed < maxSpeed  && !isBraking) {
			wheelFL.motorTorque = maxMotorTorque*driveDirection;
			wheelFR.motorTorque = maxMotorTorque*driveDirection;
		} 
		else 
		{
			wheelFL.motorTorque = 0;
			wheelFR.motorTorque = 0;
		}
	}

	private void Sensors()
	{
		RaycastHit hit;
		Vector3 sensorStartPos = transform.position;
		sensorStartPos += transform.forward * frontSensorPos.z;
		sensorStartPos += transform.up * frontSensorPos.y;	

		if(!isFrontAvoiding)
			avoidMultiplier = 0;
		
		isFrontAvoiding = false;
		isAvoiding = false;


		// front right sensor
		sensorStartPos += transform.right * frontSideSensorPos;
		if (Physics.Raycast (sensorStartPos, transform.forward, out hit, sensorLength)) {
			if(hit.collider.CompareTag("Cars")){
				isAvoiding = true;
				avoidMultiplier -= .1f;
				Debug.DrawLine (sensorStartPos, hit.point);
			}
		}

		// front right angle sensor

		 if (Physics.Raycast (sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit , sensorLength)) {
			if(hit.collider.CompareTag("Cars")){
				isAvoiding = true;
				avoidMultiplier -= .01f;
				Debug.DrawLine (sensorStartPos, hit.point);
			}
		}

		// front left sensor
		sensorStartPos -= transform.right * frontSideSensorPos * 2;
		if (Physics.Raycast (sensorStartPos, transform.forward, out hit, sensorLength)) {
			if (hit.collider.CompareTag ("Cars")) {
				isAvoiding = true;
				avoidMultiplier += .1f;
				Debug.DrawLine (sensorStartPos, hit.point);
			}
		}

		// front left angle sensor
	   if (Physics.Raycast (sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit , sensorLength)) {
			if (hit.collider.CompareTag ("Cars")) {
				isAvoiding = true;
				avoidMultiplier += .01f;
				Debug.DrawLine (sensorStartPos, hit.point);
			}
		}

		if(avoidMultiplier == 0){
			if (Physics.Raycast (sensorStartPos, transform.forward, out hit, sensorLength * 2)) {
				if (hit.collider.CompareTag ("Cars")) {
					isFrontAvoiding = true;
					isAvoiding = true;
					if (hit.normal.x < 0)
						avoidMultiplier = -.1f;
					else
						avoidMultiplier = .1f;

					Debug.DrawLine (sensorStartPos, hit.point);
				}
			}
		}			
	}

	private void SmoothSteer()
	{
		if (isAvoiding) 
		{
			targetSteerAngle = maxSteerAngle * avoidMultiplier;
		}

		wheelFL.steerAngle = Mathf.Lerp (wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * maxTurnSpeed);
		wheelFR.steerAngle = Mathf.Lerp (wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * maxTurnSpeed);
	}
}