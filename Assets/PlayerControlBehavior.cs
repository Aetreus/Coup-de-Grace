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
		if(Input.GetKey(KeyCode.DownArrow))
        {
            fb.SetElevator(0.5F);
        }
        else if(Input.GetKey(KeyCode.UpArrow))
        {
            fb.SetElevator(-0.5F);
        }
        else
        {
            fb.SetElevator(0.0F);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            fb.SetAileron(0.5F);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            fb.SetAileron(-0.5F);
        }
        else
        {
            fb.SetAileron(0.0F);
        }
    }
}
