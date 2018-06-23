using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour {

    public int teamsLeft;
    public int playersLeft;

    public long timeTillNextDrop;

    /**
     * Placeholder till we have networking setup and I can actually do some networking magic to detect players connected and alive.
    **/

	void Start () {
        teamsLeft = 0;
        playersLeft = 0;
        timeTillNextDrop = 120;
	}
	
	void Update () {
		
	}
}
