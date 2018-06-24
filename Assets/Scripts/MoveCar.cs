using BeardedManStudios.Forge.Networking.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MoveCarBehavior
{
	void Update ()
    {

        if(!networkObject.IsOwner)
        {
            transform.position = networkObject.position;
            return;
        }

        transform.position += new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ) * Time.deltaTime * 15.0f;

        Debug.Log(Input.GetAxis("Horizontal") + " - " + Input.GetAxis("Vertical"));

        networkObject.position = transform.position;
    }
}
