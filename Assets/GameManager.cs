using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;

public class GameManager : MonoBehaviour {

	void Start () {
        NetworkManager.Instance.InstantiateMoveCar();
    }
}
