using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour {

	public bool isZeroCheckpoint;

	public int count = 1;
	public int lap = 0;
	public int WaypointCheckpoint(int currentLap, int currPos)
	{
		int returnCount;
		if (lap == currentLap) {
			if (count < 15) {
				count++;
				return count-1;
			} else {
				lap++;
				count = 1;

				return count-1;
			}
		} 
		else 
		{
			if (currPos == 1 && currentLap > lap) {
				lap++;
				count = 1;
			}
			return currPos;
		}

	}
}
