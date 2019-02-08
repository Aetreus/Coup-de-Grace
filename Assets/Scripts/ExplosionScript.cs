using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour {
	public GameObject prefab;
	public int numDebris;
	public float power;
    public float dragRange = 0.4F;

    private Rigidbody myrb;

	// Use this for initialization
	void Start () {
        myrb = GetComponent<Rigidbody>();
           
		//explode();
	}

	public void explode() {
		for (int i = 0; i < numDebris; i++) {
			GameObject clone = Instantiate(prefab);
			Transform transform = clone.GetComponent<Transform>();
			Rigidbody rb = clone.GetComponent<Rigidbody>();
            rb.drag = rb.drag * Random.Range(1 + dragRange, 1 - dragRange);

			transform.position = gameObject.GetComponent<Transform>().position;
            Vector3 inherit = new Vector3(0, 0, 0);
            if (myrb != null)
                inherit = myrb.velocity;
			rb.velocity = new Vector3(Random.Range(power * -1, power), 0, Random.Range(power * -1, power)) + inherit;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
