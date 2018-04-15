using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour {

    public string hostileTag;

    private ExplosionScript es;

    // Use this for initialization
    void Start () {
        es = GetComponent<ExplosionScript>();
	}
	
	// Update is called once per frame
	void Update () {
        print(GetComponent<Rigidbody>().velocity);
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(hostileTag))
        {
            Destroy(collision.gameObject);
        }
        es.explode();
        Destroy(gameObject);
    }
}
