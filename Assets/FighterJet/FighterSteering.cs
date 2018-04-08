using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterSteering : MonoBehaviour {

    public PIDController surfaceController;

    private FighterDecisionTree tree;
    private FlightBehavior fb;
    private Rigidbody rb;

    private GameObject player;

    public float ref_speed = 200;
    public float ref_accel;
    public float dampingTime;
    private float dampingTimer;

	// Use this for initialization
	void Start ()
    {
        tree = GetComponent<FighterDecisionTree>();
        fb = GetComponent<FlightBehavior>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 accel_dir;
        if (player)
        {
            accel_dir = tree.LinearAcceleration;
        }
        else
        {
            accel_dir = Vector3.zero;
        }

        accel_dir = transform.InverseTransformVector(accel_dir);

        float surfaceDampCoeff = 1.0F;
        if (dampingTimer > 0)
        {
            surfaceDampCoeff = 1.0F - (dampingTime / dampingTimer);
            dampingTime -= Time.deltaTime;
        }

        float speedControlSense = 1.0F;
        if (rb.velocity.sqrMagnitude > ref_speed)
        {
            speedControlSense = ref_speed * ref_speed / rb.velocity.sqrMagnitude;
        }

        float rollError = Vector3.Angle(rb.transform.up, Vector3.ProjectOnPlane(accel_dir, Vector3.Cross(rb.transform.up, rb.transform.right)));
        if(Vector3.Dot(Vector3.Cross(accel_dir,rb.transform.up),Vector3.Cross(rb.transform.up,rb.transform.right)) < 0)//Get the sign of the angle difference
        {
            rollError = -rollError;
        }

        float elevatorError = Vector3.Angle(rb.transform.up, Vector3.ProjectOnPlane(accel_dir, Vector3.Cross(rb.transform.up, rb.transform.forward)));
        if (Vector3.Dot(Vector3.Cross(accel_dir, rb.transform.up), Vector3.Cross(rb.transform.up, rb.transform.forward)) < 0)//Get the sign of the angle difference
        {
            elevatorError = -elevatorError;
        }

        fb.aileron = surfaceController.Calc(rollError);
        fb.elevator = surfaceController.Calc(elevatorError);
    }
}
