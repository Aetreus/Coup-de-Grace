using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDebrisScript : MonoBehaviour {
	public float lifetime;
	private float countdown;

	// Use this for initialization
	void Start () {
		countdown = lifetime;
	}
	
	// Update is called once per frame
	void Update () {
        countdown -= Time.deltaTime;
		if (countdown < 0) {
			Destroy(gameObject);
		}
	}
}
