using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;

public class CameraManager : NetworkCameraBehavior
{

	// Use this for initialization
	void Start () {
        Camera cameraRef = GetComponent<Camera>();
        if (!networkObject.IsOwner)
            cameraRef.enabled = false;
    }
}
