using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileFlightController : MonoBehaviour {
	private MissileFiringController mFC;

	private Vector3[] targetPositions;
	private Rigidbody rb;
	private float speed = 5;

	private bool canFly = false;
	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (canFly) {
			if(Vector3.Distance(transform.position, targetPositions[count] )< 20f)
			{
				if (count < targetPositions.Length - 1) {
					count++;

					if (count == 1)
						gameObject.GetComponent<BoxCollider> ().enabled = true;
				}
			}
			rb.AddForce ((targetPositions [count] - transform.position) * speed);
			transform.rotation = Quaternion.LookRotation (rb.velocity);
		}
	}

	int count = 0;

	public void OnTriggerEnter(Collider collider)
	{
		if (canFly) {
			GameObject.FindObjectOfType<MissileFiringController> ().LastMissileCallback ();
			canFly = false;
			Destroy (gameObject);
		}
	}

	public void FireMissile(Vector3[] positions)
	{
		if (!canFly) 
		{
			canFly = true;

			targetPositions = positions;
			transform.SetParent (null);
			StartCoroutine (WaitToDestroy ());
			rb.isKinematic = false;
			rb.velocity = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody> ().velocity/2;

		}
	}

	private IEnumerator WaitToDestroy(){
		yield return new WaitForSeconds (3);
		OnTriggerEnter (null);
	}
}
