using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlightBehavior))]
public class PlayerControlBehavior : MonoBehaviour {
    FlightBehavior fb;


	// Use this for initialization
	void Start () {
        fb = GetComponent<FlightBehavior>();
	}
	
	// Update is called once per frame
	void Update () {
		fb.SetElevator(Input.GetAxis("Elevator"));

		fb.SetAileron(Input.GetAxis("Aileron"));
    }
}
