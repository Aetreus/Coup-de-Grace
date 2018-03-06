using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNSpawner : MonoBehaviour {

    public GameObject spawned;
    public Vector3 offset;
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
        GameObject created = Instantiate(spawned, transform.position + offset, transform.rotation);
        created.GetComponent<PropNav>().Target = target;
    }
}
