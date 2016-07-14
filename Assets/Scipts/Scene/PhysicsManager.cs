using UnityEngine;
using System.Collections;

public class PhysicsManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Physics.gravity = -Vector3.up * 98;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
