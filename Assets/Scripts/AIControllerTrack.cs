using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AIControllerTrack : MonoBehaviour {

	public List<Transform> points = new List<Transform>();

	private Vector3 currentPoint;
	private Vector3 previousPoint;

	public int previousCount = 0;

	[Header("Wheels Rotation")]
	public Transform frontLeftWheel;
	public Transform frontRightWheel;
	public Transform rearRightWheel;
	public Transform rearLeftWheel;
	[Header("Wheels Turn")]
	public Transform lfWheelContainer;
	public Transform rfWheelContainer;

	public Text mySpeedText;

	private Rigidbody carRigidbody;

	private Transform[] wheelTransform;
	private Transform carTransform;


		// Use this for initialization
	void Start () {
		SetWaypoints ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (transform.position, currentPoint) < 1) 
		{
			GetNextPoint ();
		}

	}

	private void GetNextPoint()
	{
		if (previousCount < points.Count-1) {
			currentPoint = points [previousCount].position;
			previousPoint = points [points.Count - 1].position;
			previousCount++;
		} 
		else 
		{
			currentPoint = points [previousCount].position;
			previousPoint = points [previousCount-1].position;
			previousCount = 0;
		}
	
	}

	private void SetWaypoints()
	{
			currentPoint = points [0].position;
			previousPoint = points [points.Count - 1].position;
	}

}