using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class FlightBehavior : MonoBehaviour {
    Rigidbody rb;

    const float airDensity = 1.225F; //kg/m^2

    public float dragCoeff = 0.04F;

    public float wingArea = 56; //m^2

    public float maxThrust = 60000;

    public float pitchingMomentCoeffIntercept = 0.05F;

    public float pitchingMomentCoeffSlope = -0.01F;

    float AoA;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate ()
    {

        float thrustMagnitude = maxThrust;
        Vector3 thrust = rb.transform.forward * thrustMagnitude;
        rb.AddForce(thrust, ForceMode.Force);

        AoA = Vector3.Angle(rb.velocity, rb.transform.forward);
        float liftMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude,2) * CalculateLiftCoeff();
        Vector3 lift = new Vector3(0, liftMagnitude, 0);//TODO:Should be seperating lift from induced drag. 
        rb.AddForce(lift, ForceMode.Force);

        float dragMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude,2) * dragCoeff;
        Vector3 drag = rb.velocity.normalized * -1 * dragMagnitude;
        rb.AddForce(drag, ForceMode.Force);


        //Unity is left-handed, so all moments need to be inverted to keep constants in line with reference values
        float cMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * CalculatePitchingCoeff() * -1;
        rb.AddRelativeTorque(cMMagnitude, 0, 0);
    }

    float CalculateLiftCoeff()
    {
        if(AoA > 25)//Very lazy stall condition
        {
            return 2.4F - 0.11F * (AoA - 25);
        }
        else if(AoA > 20)
        {
            return 2.4F;
        }
        else if(AoA < -10)
        {
            return -0.9F;
        }
        else
        {
            return 0.11F * AoA + 0.2F;
        }
    }

    float CalculatePitchingCoeff()
    {
        return pitchingMomentCoeffIntercept + AoA * pitchingMomentCoeffSlope;
    }
}
