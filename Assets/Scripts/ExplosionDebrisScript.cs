using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDebrisScript : MonoBehaviour {
	public float gravity;
	public float lifetime;
	private float countdown;

	// Use this for initialization
	void Start () {
		countdown = lifetime;
	}
	
	// Update is called once per frame
	void Update () {
		countdown -= 1;
		if (countdown == 0) {
			Destroy(gameObject);
		}
		gameObject.GetComponent<Rigidbody>().AddForce(Vector3.down * gravity);
	}
}
