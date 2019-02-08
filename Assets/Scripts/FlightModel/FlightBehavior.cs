using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class FlightBehavior : MonoBehaviour {
    Rigidbody rb;

    public Vector3 inertiaTensor = new Vector3(0,0,0);

    const float slAirDensity = 1.225F; //kg/m^2

    private Dictionary<float,float> airDensitySetpoints = new Dictionary<float, float>() {
        { -10000, 3.3F },
        { -1000, 1.347F},
        { 0,1.225F},
        { 1000,1.112F},
        { 2000,1.007F},
        { 3000,0.9093F },
        { 4000,0.8194F},
        { 5000,0.7364F },
        { 6000,0.6601F },
        { 7000,0.5900F },
        {8000,0.5258F },
        {9000,0.4671F },
        {10000,0.4135F },
        {15000,0.1948F },
        {20000,0.08891F },
        {25000,0.04008F } };

    public float dragCoeff = 0.04F;

    public float wingArea = 56; //m^2

    public float staticThrust = 60000;

    //Aerodynamic forces that damp out roll and yaw based on the aircrafts velocity as well.
    public float rollDampeningCoeff = 0.05F;

    public float yawDampeningCoeff = 0.1F;

    private float elevatorSetting = 0;//These values are the current settings of the control surfaces, from 1.0 to -1.0

    private float aileronSetting = 0;

    private float rudderSetting = 0;

    private float throttleSetting = 1;
    //These coefficients drive how large the moments that the control surfaces apply to the aircraft are.
    public float aileronControlCoeff = 0.25F;

    public float elevatorControlCoeff = 0.3F;

    public float rudderControlCoeff = 0.1F;

    //Drive how strong lift is in the vertical direction based on AoA. Usually asymmetric biased towards positive lift.
    public List<Vector2> liftCoeffSetpoints = new List<Vector2> { new Vector2(-180,0.0F),new Vector2(-35, -0.4F), new Vector2(-20,-2.0F),new Vector2(0, 0.2F),new Vector2(25 ,2.81F),new Vector2(30,2.61F),new Vector2(45,0.4F),new Vector2(180,0.0F)};

    //Drive how strong lift is in the sideways direction based on sideslip(much lower for typical aircraft).
    public List<Vector2> crossLiftCoeffSetpoints = new List<Vector2> { new Vector2(-180, 0.0F), new Vector2(-35, -0.05F), new Vector2(-20, -0.25F), new Vector2(20, 0.25F), new Vector2(35, -0.05F), new Vector2(180, 0.0F) };

    //Pitching moment coefficient is an aircraft's tendency to center itself vertically(i.e. to fly in the direction it's pointing). Slope should be negative over the stable range.
    public List<Vector2> pitchingMomentSetpoints = new List<Vector2> { new Vector2(-180, 0.0F), new Vector2(-50, 0.1F), new Vector2(-35, 0.285F), new Vector2(0, 0.005F), new Vector2(35, -0.275F), new Vector2(50, -0.1F), new Vector2(180, 0) };

    //Yaw moment coefficient is similar, but makes the aircraft stable in yaw instead of pitch. Yaw moment coefficient slope should be negative over the stable range.
    public List<Vector2> yawMomentSetpoints = new List<Vector2> { new Vector2(-180, 0.0F), new Vector2(-50, 0.1F), new Vector2(-35, 0.35F), new Vector2(0, 0.000F), new Vector2(35, -0.35F), new Vector2(50, -0.1F), new Vector2(180, 0) };

    public AnimationCurve liftCoeff = new AnimationCurve();

    public void OnBeforeSerialize()
    {
        foreach(Vector2 setpoint in liftCoeffSetpoints)
        {
            liftCoeff.AddKey(setpoint.x, setpoint.y);
        }
    }

    public void OnAfterDeserialize()
    {
        foreach (Vector2 setpoint in liftCoeffSetpoints)
        {
            liftCoeff.AddKey(setpoint.x, setpoint.y);
        }
    }

    private float AoA = 0.0F;

    private float sideslip;

    private float airDensity;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();

        if(!inertiaTensor.Equals(new Vector3(0,0,0)))
        {
            rb.inertiaTensor = inertiaTensor;
        }
        else
        {
            inertiaTensor = rb.inertiaTensor;
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float elevator { get { return elevatorSetting; }
    set {
            if (value <= 1.0F && value >= -1.0F)
            {
                elevatorSetting = value;
            }
            else if (value > 1.0F)
            {
                elevatorSetting = 1.0F;
            }
            else
            {
                elevatorSetting = -1.0F;
            }
        }
    }

    public float aileron { get { return aileronSetting; }
    set {
        if (value <= 1.0F && value >= -1.0F)
        {
            aileronSetting = value;
        }
        else if (value > 1.0F)
        {
            aileronSetting = 1.0F;
        }
        else
        {
            aileronSetting = -1.0F;
        }
    } }

    public float rudder {  get { return rudderSetting; }
    set
    {
        if (value <= 1.0F && value >= -1.0F)
        {
            rudderSetting = value;
        }
        else if (value > 1.0F)
        {
            rudderSetting = 1.0F;
        }
        else
        {
            rudderSetting = -1.0F;
        }
        
    }
    }

    public float throttle { get { return throttleSetting; }
    set
        {
            if (value <= 1.0F && value >= 0.0F)
            {
                throttleSetting = value;
            }
            else if (value > 1.0F)
            {
                throttleSetting = 1.0F;
            }
            else
            {
                throttleSetting = 0.0F;
            }
        }
    }

    public float aoa { get { return AoA; } }

    public float slip { get { return sideslip; } }

    public float airspeed { get { return GetComponent<Rigidbody>().velocity.magnitude; } }

    void FixedUpdate ()
    {
        airDensity = AirDensityAdjusted();
        float thrustMagnitude = staticThrust;
        Vector3 thrust = rb.transform.forward * thrustMagnitude * throttleSetting;
        rb.AddForce(thrust, ForceMode.Force);

        //AoA is the portion of the angle between facing and velocity vector that is in the plane between the up and forward vectors of facing
        AoA = Vector3.Angle(rb.transform.forward, Vector3.ProjectOnPlane(rb.velocity,Vector3.Cross(rb.transform.forward,rb.transform.up)));
        if(Vector3.Dot(Vector3.Cross(rb.velocity,rb.transform.forward),Vector3.Cross(rb.transform.forward,rb.transform.up)) < 0)//Get the sign of the angle difference
        {
            AoA = -AoA;
        }

        //
        sideslip = Vector3.Angle(rb.transform.forward, Vector3.ProjectOnPlane(rb.velocity, Vector3.Cross(rb.transform.right, rb.transform.forward)));
        if (Vector3.Dot(Vector3.Cross(rb.velocity, rb.transform.forward), Vector3.Cross(rb.transform.right, rb.transform.forward)) < 0)//Get the sign of the angle difference
        {
            sideslip = -sideslip;
        }

        float liftMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude,2) * CalculateLiftCoeff();
        Vector3 liftDirection = Vector3.Cross(rb.velocity, rb.transform.right).normalized;
        Vector3 lift = liftDirection * liftMagnitude;
        rb.AddForce(lift, ForceMode.Force);

        float crossLiftMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * CalculateCrossLiftCoeff();
        Vector3 crossLiftDirection = Vector3.Cross(rb.velocity, rb.transform.up).normalized;
        Vector3 crossLift = crossLiftDirection * crossLiftMagnitude;
        rb.AddForce(crossLift, ForceMode.Force);

        float dragMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * CalculateDragCoeff();
        Vector3 drag = rb.velocity.normalized * -1 * dragMagnitude;
        rb.AddForce(drag, ForceMode.Force);

        //Unity is left-handed, so all moments need to be inverted to keep constants in line with reference values
        float cMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * CalculatePitchingCoeff() * -1;
        rb.AddRelativeTorque(cMMagnitude, 0, 0);

        float yMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * CalculateYawCoeff() * -1;
        rb.AddRelativeTorque(0, yMMagnitude, 0);

        Vector3 localAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity);//Why is there no rb.GetRelativeAngularVelocity()?
        float rollDampMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * localAngularVelocity.z * rollDampeningCoeff * -1;
        rb.AddRelativeTorque(0, 0, rollDampMagnitude);

        float yawDampMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * localAngularVelocity.y * yawDampeningCoeff * -1;
        rb.AddRelativeTorque(0, yawDampMagnitude, 0);

        float eSigMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * elevatorControlCoeff * elevatorSetting * -1;
        rb.AddRelativeTorque(eSigMMagnitude, 0, 0);

        float rSigMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * rudderControlCoeff * rudderSetting * -1;
        rb.AddRelativeTorque(0, rSigMMagnitude, 0);

        float aSigMMMagnitude = (1F / 2) * airDensity * wingArea * (float)Math.Pow((double)rb.velocity.magnitude, 2) * aileronControlCoeff * aileronSetting * -1;
        rb.AddRelativeTorque(0, 0, aSigMMMagnitude);
    }

    float CalculateYawCoeff()
    {
        return InterpolateSetpoints(sideslip, yawMomentSetpoints);
    }

    float CalculateLiftCoeff()
    {
        /*
        if (AoA >= -20 && AoA < 25)//Still lazy but better stall condition.
        {
            return 0.11F * AoA + 0.2F;
        }
        else if (AoA >= 25 && AoA < 30)
        {
            return 2.8F;
        }
        else if (AoA < -20 && AoA >= -41)
        {
            return -2.4F - 0.11F * (AoA + 20);
        }
        else if (AoA >= 30 && AoA < 45)
        {
            return 2.8F - 0.2F * (AoA - 29);
        }
        else
            return 0.0F;
            */
        return InterpolateSetpoints(AoA, liftCoeffSetpoints);
    }

    float CalculateCrossLiftCoeff()
    {
        return InterpolateSetpoints(sideslip, crossLiftCoeffSetpoints);
    }

    float CalculateDragCoeff()
    {
        return Math.Min(dragCoeff + 0.025F * Math.Abs(AoA), 2.0F);
    }

    float CalculatePitchingCoeff()
    {
        return InterpolateSetpoints(AoA, pitchingMomentSetpoints);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Terrain"))
        {
            if (gameObject.GetComponent<HPManager>() != null)
            {
                gameObject.GetComponent<HPManager>().Die();
            }
            else
                Destroy(gameObject);
        }
    }

    //Generates an interpolated point from the given setpoints. Setpoints must be sequential and input must be in the defined range.
    float InterpolateSetpoints(float input,List<Vector2> setpoints)
    {
        for(int i = 1; i < setpoints.Count; i++)
        {
            //Linear interpolation
            if(setpoints[i-1].x <= input && setpoints[i].x >= input)
            {
                return setpoints[i - 1].y + (input - setpoints[i-1].x) * (setpoints[i].y - setpoints[i - 1].y) / (setpoints[i].x - setpoints[i - 1].x);
            }
        }
        throw new ArgumentException("Input not within setpoint range");
    }

    //Calculates air density at current altitude based on internal setpoint map.
    float AirDensityAdjusted()
    {
        List<float> keys = new List<float>(airDensitySetpoints.Keys);
        for (int i = 1; i < keys.Count; i++)
        {
            //Linear interpolation
            if (keys[i-1] <= rb.position.y && keys[i] >= rb.position.y)
            {
                return airDensitySetpoints[keys[i - 1]] + (rb.position.y - keys[i-1]) * (airDensitySetpoints[keys[i]] - airDensitySetpoints[keys[i - 1]]) / (keys[i] - keys[i - 1]);
            }
        }
        throw new InvalidOperationException("Altitude exceeded definied pressure range.");
    }
}
