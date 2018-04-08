using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNSpawner : MonoBehaviour {

    public GameObject spawned;
    public Vector3 offset;
    public Vector3 initial;

	// Use this for initialization
	protected void Start () {
	}
	
	// Update is called once per frame
	protected void Update () {
		
	}

    public virtual void Create(GameObject target)
    {
        GameObject created = Instantiate(spawned, transform.TransformPoint(offset), transform.rotation);
        created.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + transform.localToWorldMatrix.MultiplyVector(initial);
        created.GetComponent<PropNav>().Target = target;
        created.GetComponent<ProximityExplodeScript>().hostileTag = "Enemy";
    }
}
