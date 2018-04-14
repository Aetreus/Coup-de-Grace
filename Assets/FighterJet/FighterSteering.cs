using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(FlightBehavior))]
public class FighterSteering : MonoBehaviour {

    //public PIDController pitchController;
    //public PIDController rollController;

    public PIDController aileronController;
    public PIDController elevatorController;
    public PIDController yawController;
    public PIDController throttleController;
    
    private FlightBehavior fb;
    private Rigidbody rb;

    private GameObject player;

    public float fullTurnAngle;
    public float ref_speed = 200;
    public float tolerance = 2;

    public Vector3 baseDir = Vector3.up;
    public Vector3 facingDir = new Vector3(0,0,1);
    public Vector3 upDir = new Vector3(0,1,0);
    public float targetVel = 200;
    public float minAirspeed = 80;
    public float stallLimit = -5;

    private float _rollError;
    private float _pitchError;

	// Use this for initialization
	void Start ()
    {
        fb = GetComponent<FlightBehavior>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update ()
    {
        float angle = Vector3.Angle(transform.forward, facingDir);
        float upAngle = Vector3.Angle(transform.forward, upDir);

        Vector3 accel_vec = Vector3.zero;
        if (player)
        {
        }
        else
        {
            accel_vec = Vector3.zero;
        }

        Debug.DrawRay(transform.position, facingDir * 20000, Color.red);
        Debug.DrawRay(transform.position, Vector3.ProjectOnPlane(upDir, Vector3.Cross(rb.transform.up, rb.transform.right)) * 20000, Color.blue);

        //Debug.DrawRay(transform.position, local_accel_vec * 20000, Color.blue);
        /*
        float speedControlSense = 1.0F;
        if (rb.velocity.sqrMagnitude > ref_speed)
        {
            speedControlSense = ref_speed * ref_speed / rb.velocity.sqrMagnitude;
        }
        */

        float scaleFactor = (float)Math.Pow(1 - (upAngle / fullTurnAngle),1);
        if (upAngle < fullTurnAngle)
            upDir = upDir + (baseDir - upDir) * scaleFactor;
        _rollError = Vector3.Angle(rb.transform.up, Vector3.ProjectOnPlane(upDir, Vector3.Cross(rb.transform.up, rb.transform.right)));
        if(Vector3.Dot(Vector3.Cross(upDir,rb.transform.up),Vector3.Cross(rb.transform.up,rb.transform.right)) < 0)//Get the sign of the angle difference
        {
            _rollError = -_rollError;
        }

        _pitchError = Vector3.Angle(rb.transform.forward, Vector3.ProjectOnPlane(facingDir, Vector3.Cross(rb.transform.up, rb.transform.forward)));
        if (Vector3.Dot(Vector3.Cross(facingDir, rb.transform.forward), Vector3.Cross(rb.transform.up, rb.transform.forward)) < 0)//Get the sign of the angle difference
        {
            _pitchError = -_pitchError;
        }

        _pitchError = Vector3.Angle(rb.transform.forward, Vector3.ProjectOnPlane(facingDir, Vector3.Cross(rb.transform.right, rb.transform.forward)));
        if (Vector3.Dot(Vector3.Cross(facingDir, rb.transform.forward), Vector3.Cross(rb.transform.right, rb.transform.forward)) < 0)//Get the sign of the angle difference
        {
            _pitchError = -_pitchError;
        }


        if (Math.Abs(_rollError) > 90)
        {
            _rollError = Math.Sign(_rollError) * 180 - _rollError;
        }
        if(Math.Abs(angle) > tolerance)
            fb.aileron = -aileronController.Calc(_rollError);
        if(Math.Abs(angle) > tolerance)
            fb.elevator = elevatorController.Calc(_pitchError);
        fb.throttle = throttleController.Calc(targetVel - fb.airspeed);
        fb.rudder = yawController.Calc(fb.slip);
    }
}
