using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;

public class GameManager : MonoBehaviour {

    public GameObject car;

	void Start ()
    {
        if(Application.isEditor)
        {
            GameObject wreckClone = (GameObject)Instantiate(car, transform.position, transform.rotation);
        }
        else
        {
            NetworkManager.Instance.InstantiateMoveCar();
        }
    }
}
