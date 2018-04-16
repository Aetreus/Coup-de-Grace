using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PIDController{

    public float propConstant;
    public float integConstant;
    public float derivConstant;

    public float output;

    private float prevErr;
    private float integ;

    public float Calc(float error)
    {
        float deriv = (error - prevErr) / Time.deltaTime;
        integ += error * Time.deltaTime;
        output = propConstant * error + integConstant * integ + deriv * derivConstant;
        prevErr = error;
        return output;
    }

}
