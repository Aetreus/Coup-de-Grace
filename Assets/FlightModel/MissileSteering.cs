using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSteering : MonoBehaviour {

    private PropNav prop_nav;
    private FlightBehavior fb;

    public float ref_accel;

	// Use this for initialization
	void Start () {
        prop_nav = GetComponent<PropNav>();
        fb = GetComponent<FlightBehavior>();
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 latex = prop_nav.Latex;

        Vector3 local_accel = transform.InverseTransformVector(latex);

        fb.throttle = local_accel.z / ref_accel;
        fb.rudder = local_accel.x / ref_accel;
        fb.elevator = local_accel.y / ref_accel;
    }
}
