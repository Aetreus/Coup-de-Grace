using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour {
	public GameObject prefab;
	public int numDebris;
	public float power;

	// Use this for initialization
	void Start () {
		explode();
	}

	public void explode() {
		for (int i = 0; i < numDebris; i++) {
			GameObject clone = Instantiate(prefab);
			Transform transform = clone.GetComponent<Transform>();
			Rigidbody rb = clone.GetComponent<Rigidbody>();

			transform.position = gameObject.GetComponent<Transform>().position;
			rb.velocity = new Vector3(Random.Range(power * -1, power), Random.Range(power * -1, power), Random.Range(power * -1, power));
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
