using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Path : MonoBehaviour {
	
	public List<Transform> points = new List<Transform>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	
		for (int i = 0; i < points.Count; i++) 
		{
			Vector3 current = points [i].position;
			Vector3 previous = Vector3.zero;

			if (i > 0) {
				previous = points [i - 1].position;
			} 
			else if (i == 0 && points.Count > 1) {
				previous = points [points.Count - 1].position;
			}

			Debug.DrawLine (current, previous, Color.white);

		}
	}
}
