using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNSpawner : MonoBehaviour {

    public GameObject spawned;
    public Vector3 offset;
    public Vector3 initial;
    PropNav pn;

	// Use this for initialization
	void Start () {
        pn = spawned.GetComponent<PropNav>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Create(GameObject target)
    {
        GameObject created = Instantiate(spawned, transform.TransformPoint(offset), transform.rotation);
        created.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + transform.localToWorldMatrix.MultiplyVector(initial);
        created.GetComponent<PropNav>().Target = target;
    }
}
