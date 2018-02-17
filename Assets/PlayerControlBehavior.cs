﻿using System.Collections;
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
		if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            fb.SetElevator(0.5F);
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            fb.SetElevator(-0.5F);
        }
	}
}
