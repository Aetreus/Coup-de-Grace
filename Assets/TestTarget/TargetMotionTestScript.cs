using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMotionTestScript : MonoBehaviour {

    private Vector3 velocity;

    // Use this for initialization
    void Start () {
        velocity = new Vector3(2, 2, 2);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(velocity * Time.deltaTime);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
}
