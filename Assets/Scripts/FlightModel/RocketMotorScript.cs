using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketMotorScript : MonoBehaviour {

    public float thrust = 0;
    public float duration = 0;

    private float lifetime;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        lifetime = duration;
	}
	
	// Update is called once per frame
	void Update () {
        rb.AddForce(transform.forward * thrust, ForceMode.Force);
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
            thrust = 0;
	}
}
