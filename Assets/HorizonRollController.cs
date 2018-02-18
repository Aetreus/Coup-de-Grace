using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonRollController : MonoBehaviour {

    public GameObject reference;
    public Vector3 rotateAxis = new Vector3(0, 1, 0);

    private Quaternion baseRotation;

	// Use this for initialization
	void Start () {
        baseRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        //float pitch = Vector3.Angle(Vector3.ProjectOnPlane(reference.transform.forward, new Vector3(0, 1, 0)), reference.transform.forward);

        float roll = Vector3.Angle(reference.transform.up, Vector3.ProjectOnPlane(new Vector3(0, 1, 0), Vector3.Cross(reference.transform.up, reference.transform.right)));
        if(Vector3.Dot(Vector3.Cross(reference.transform.up, Vector3.ProjectOnPlane(new Vector3(0, 1, 0), Vector3.Cross(reference.transform.up, reference.transform.right))),Vector3.Cross(reference.transform.up, reference.transform.right)) < 0)
        {
            roll = -roll;
        }
        transform.rotation = baseRotation * Quaternion.AngleAxis(roll, rotateAxis);
	}
}
