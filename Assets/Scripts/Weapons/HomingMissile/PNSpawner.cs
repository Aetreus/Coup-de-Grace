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

    public virtual GameObject Create(GameObject target)
    {
        GameObject created = Instantiate(spawned, transform.TransformPoint(offset), transform.rotation);
        GameObject sel = gameObject;
        Rigidbody rb = null;
        while (rb == null)
        {
            rb = sel.GetComponent<Rigidbody>();
            if (rb == null)
                sel = sel.transform.parent.gameObject;
        }
        created.GetComponent<Rigidbody>().velocity = rb.velocity + transform.localToWorldMatrix.MultiplyVector(initial);
        created.GetComponent<PropNav>().Target = target;
        created.GetComponent<ProximityExplodeScript>().hostileTag = target.tag;
        return created;
    }
}
