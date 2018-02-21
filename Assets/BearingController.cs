using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearingController : MonoBehaviour {

	public GameObject reference;

	public Vector3 translateBase;//Translation amount corresponding to 1 degree pitch

	private Vector3 baseLoc;

	// Use this for initialization
	void Start () {
		baseLoc = transform.localPosition;
	}

	// Update is called once per frame
	void Update () {
		float bearing = Vector3.Angle(Vector3.ProjectOnPlane(reference.transform.forward, new Vector3(0, 1, 0)), new Vector3(0,0,1));
		if(Vector3.Dot(Vector3.Cross(Vector3.ProjectOnPlane(reference.transform.forward, new Vector3(0, 1, 0)),new Vector3(0,0,1)),new Vector3(0,1,0)) > 0)
		{
			bearing = -bearing;
		}

		transform.localPosition = baseLoc + translateBase * bearing;
	}
}
