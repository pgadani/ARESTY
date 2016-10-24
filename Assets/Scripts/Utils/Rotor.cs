using UnityEngine;
using System.Collections;

public class Rotor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.Rotate(transform.forward, 1, Space.World);
	}
}
