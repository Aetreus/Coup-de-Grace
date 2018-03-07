﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (FlightBehavior)),RequireComponent(typeof (Rigidbody))]
public class PropNav : MonoBehaviour {

    public float N = 3;
    public float ref_accel;
    public float ref_speed = 200;

    public float damage;
    public float fueltime;

    private GameObject target = null;
    private Vector3 last_pos;
    private Vector3 target_last_pos;
    
    private FlightBehavior fb;

    private Rigidbody rb;

    // Use this for initialization
    void Start () {
        last_pos = transform.position;
        fb = GetComponent<FlightBehavior>();
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 latax;

        if (target)
        {
            Vector3 range = target.transform.position - transform.position;

            Vector3 missile_vel = GetComponent<Rigidbody>().velocity;
            Vector3 relative_vel;
            if (target.GetComponent<Rigidbody>() == null)
            {
                relative_vel = -missile_vel;   
            }
            else
            {
                relative_vel = target.GetComponent<Rigidbody>().velocity - missile_vel;
            }

            Vector3 rotation_vec = Vector3.Cross(range, relative_vel) / Vector3.Dot(range, range);

            Vector3 term1 = -N * relative_vel.magnitude * missile_vel.normalized;

            latax = Vector3.Cross(term1, rotation_vec);
        }
        else
        {
            latax = Vector3.zero;
        }

        Vector3 local_accel = transform.InverseTransformVector(latax);

        print(local_accel);

        if(fueltime > 0)
        {
            fb.throttle = 1.0F;
        }
        else
        {
            fb.throttle = 0.0F;
        }
        float speedControlSense = 1.0F;
        if(rb.velocity.sqrMagnitude > ref_speed)
        {
            speedControlSense = ref_speed * ref_speed / rb.velocity.sqrMagnitude;
        }

        fb.rudder = (-local_accel.x / ref_accel) * speedControlSense;
        fb.elevator = (local_accel.y / ref_accel) * speedControlSense;

        fueltime -= Time.deltaTime;
    }

    public GameObject Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
            target_last_pos = target.transform.position;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (target && collision.gameObject == target)
        {
            HPManager hp = collision.gameObject.GetComponent<HPManager>();
            hp.Damage(damage);
        }

        Destroy(gameObject);
    }
}
