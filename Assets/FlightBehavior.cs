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

    //Pitching moment coefficient is an aircraft's tendency to center itself vertically(i.e. to fly in the direction it's pointing)
    public float pitchingMomentCoeffIntercept = 0.05F;//Positive values means the plane has a certain amount of tendency to fly nose-up(it's nose is pulled up when flying straight).

    public float pitchingMomentCoeffSlope = -0.01F;//This must be negative for stability.

    //Yaw moment coefficient is similar, but makes the aircraft stable in yaw instead of pitch.
    public float yawMomentCoeffIntercept = 0.0F;//Should be zero so that the aircraft faces the direction it is flying.

    public float yawMomentCoeffSlope = -0.05F;//This value must be negative, but its magnitude is less important.

    private float elevatorSetting = 0;//These values are the current settings of the control surfaces, from 1.0 to -1.0

    private float aileronSetting = 0;

    private float rudderSetting = 0;

    public float aileronControlCoeff = 0.25F;//These coefficients drive how large the moments that the control surfaces apply to the aircraft are.

    public float elevatorControlCoeff = 0.3F;

    public float rudderControlCoeff = 0.1F;

    private float AoA;

    private float sideslip;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetElevator (float desired)
    {
        if(desired <= 1.0F && desired >= -1.0F)
        {
            elevatorSetting = desired;
        }
        else if(desired > 1.0F)
        {
            elevatorSetting = 1.0F;
        }
        else
        {
            elevatorSetting = -1.0F;
        }
    }

    public float GetElevator ()
    {
        return elevatorSetting;
    }

    public void SetAileron (float desired)
    {
        if (desired <= 1.0F && desired >= -1.0F)
        {
            aileronSetting = desired;
        }
        else if (desired > 1.0F)
        {
            aileronSetting = 1.0F;
        }
        else
        {
            aileronSetting = -1.0F;
        }
    }

    public float GetAileron()
    {
        return aileronSetting;
    }

    public void SetRudder (float desired)
    {
        if (desired <= 1.0F && desired >= -1.0F)
        {
            rudderSetting = desired;
        }
        else if (desired > 1.0F)
        {
            rudderSetting = 1.0F;
        }
        else
        {
            rudderSetting = -1.0F;
        }
    }

    public float GetRudder()
    {
        return rudderSetting;
    }

    void FixedUpdate ()
    {

        float thrustMagnitude = maxThrust;
        Vector3 thrust = rb.transform.forward * thrustMagnitude;
        rb.AddForce(thrust, ForceMode.Force);

        //AoA is the portion of the angle between facing and velocity vector that is in the plane between the up and forward vectors of facing
        AoA = Vector3.Angle(rb.velocity, Vector3.ProjectOnPlane(rb.transform.forward,Vector3.Cross(rb.transform.forward,rb.transform.up)));
        if(Vector3.Dot(Vector3.Cross(rb.velocity,rb.transform.forward),Vector3.Cross(rb.transform.forward,rb.transform.up)) < 0)//Get the sign of the angle difference
        {
            AoA = -AoA;
        }

        //
        sideslip = Vector3.Angle(rb.velocity, Vector3.ProjectOnPlane(rb.transform.forward, Vector3.Cross(rb.transform.right, rb.transform.forward)));
        if (Vector3.Dot(Vector3.Cross(rb.velocity, rb.transform.forward), Vector3.Cross(rb.transform.right, rb.transform.up)) > 0)//Get the sign of the angle difference
        {
            sideslip = -sideslip;
        }

        float liftMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude,2) * CalculateLiftCoeff();
        Vector3 lift = new Vector3(0, liftMagnitude, 0);//TODO:Should be seperating lift from induced drag. 
        rb.AddForce(lift, ForceMode.Force);

        float dragMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude,2) * dragCoeff;
        Vector3 drag = rb.velocity.normalized * -1 * dragMagnitude;
        rb.AddForce(drag, ForceMode.Force);

        //Unity is left-handed, so all moments need to be inverted to keep constants in line with reference values
        float cMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * CalculatePitchingCoeff() * -1;
        rb.AddRelativeTorque(cMMagnitude, 0, 0);

        float yMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * CalculateYawCoeff() * -1;
        rb.AddRelativeTorque(0, yMMagnitude, 0);


        float eSigMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * elevatorControlCoeff * elevatorSetting * -1;
        rb.AddRelativeTorque(eSigMMagnitude, 0, 0);

        float rSigMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * rudderControlCoeff * rudderSetting * -1;
        rb.AddRelativeTorque(0, rSigMMagnitude, 0);

        float aSigMMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * aileronControlCoeff * aileronSetting * -1;
        rb.AddRelativeTorque(0, 0, aSigMMMagnitude);
    }

    float CalculateYawCoeff()
    {
        return yawMomentCoeffIntercept + sideslip * yawMomentCoeffSlope;
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
