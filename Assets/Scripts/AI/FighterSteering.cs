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
    public PIDController pitchController;
    public PIDController yawController;
    public PIDController throttleController;
    
    private FlightBehavior fb;
    private Rigidbody rb;
    

    public float fullTurnAngle;
    public float ref_speed = 200;
    public float turnTolerance = 20;
    public float tolerance = 4;

    public Vector3 baseDir = Vector3.up;
    public Vector3 facingDir = new Vector3(0,0,1);
    public Vector3 upDir = new Vector3(0,1,0);
    public float targetVel = 200;
    public float minAirspeed = 80;
    public float aoaMax = 25;
    public float aoaMin = -20;

    private float _rollError;
    private float _pitchHeadingError;
    private float _pitchAngle;
    private float _yawError;

	// Use this for initialization
	void Start ()
    {
        fb = GetComponent<FlightBehavior>();
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        float angle = Vector3.Angle(transform.forward, facingDir);
        float upAngle = Vector3.Angle(transform.forward, upDir);

        Vector3 accel_vec = Vector3.zero;

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

        //Calculate difference between set up/forward vectors and current course.
        _rollError = Vector3.Angle(Vector3.ProjectOnPlane(rb.transform.up,rb.velocity), Vector3.ProjectOnPlane(facingDir, rb.velocity));
        if(Vector3.Dot(Vector3.Cross(facingDir,rb.transform.up),Vector3.Cross(rb.transform.up,rb.transform.right)) < 0)//Get the sign of the angle difference
        {
            _rollError = -_rollError;
        }

        _pitchHeadingError = Vector3.Angle(rb.velocity, Vector3.ProjectOnPlane(facingDir, Vector3.Cross(rb.transform.up, rb.velocity.normalized)));
        if (Vector3.Dot(Vector3.Cross(facingDir, rb.transform.forward), Vector3.Cross(rb.transform.up, rb.transform.forward)) < 0)//Get the sign of the angle difference
        {
            _pitchHeadingError = -_pitchHeadingError;
        }

        //Get current pitch angle
        _pitchAngle = fb.aoa;
        
        /*
        _yawError = Vector3.Angle(rb.transform.forward, Vector3.ProjectOnPlane(facingDir, Vector3.Cross(rb.transform.right, rb.transform.forward)));
        if (Vector3.Dot(Vector3.Cross(facingDir, rb.transform.forward), Vector3.Cross(rb.transform.right, rb.transform.forward)) < 0)//Get the sign of the angle difference
        {
            _yawError = -_yawError;
        }
        */

        /*
        if (Math.Abs(_rollError) > 90)
        {
            _rollError = Math.Sign(_rollError) * 180 - _rollError;
        }
        */
        //If the aircraft isn't facing in the ocrrect orientation for a turn but wants to turn, roll it into the correct orientation
        if (Math.Abs(angle) > tolerance && Math.Abs(_rollError) > turnTolerance)
        {
            fb.aileron = -aileronController.Calc(_rollError);
            fb.elevator = 0;
            elevatorController.Reset();
        }
        //If it is in the correct orientation pitch it and continue keeping it in the correct orientation
        else if(Math.Abs(angle) > tolerance)
        {
            fb.aileron = -aileronController.Calc(_rollError);
            float desiredPitch = pitchController.Calc(_pitchHeadingError);
            //Desired pitch is clamped to stall limit values
            Mathf.Clamp(desiredPitch, aoaMin, aoaMax);
            //This feeds the controller to set the elevators to align the aircraft with the desired pitch angle
            fb.elevator = elevatorController.Calc(desiredPitch - _pitchAngle);
        }
        //If the aircraft is in the correct orientation, try to align it with the desired up vector
        else
        {
            _rollError = Vector3.Angle(Vector3.ProjectOnPlane(rb.transform.up, rb.velocity), Vector3.ProjectOnPlane(upDir, rb.velocity));
            if (Vector3.Dot(Vector3.Cross(upDir, rb.transform.up), Vector3.Cross(rb.transform.up, rb.transform.right)) < 0)//Get the sign of the angle difference
            {
                _rollError = -_rollError;
            }
            fb.aileron = -aileronController.Calc(_rollError);
            elevatorController.Reset();
        }
        fb.throttle = throttleController.Calc(targetVel - fb.airspeed);
        //Cancel out any slip with the rudder
        fb.rudder = yawController.Calc(fb.slip);
    }
}
