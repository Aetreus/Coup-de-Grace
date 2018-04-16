using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMSpawner : PNSpawner {


    public override void Create(GameObject target)
    {
        GameObject created = Instantiate(spawned, transform.TransformPoint(offset), transform.rotation);
        created.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + transform.localToWorldMatrix.MultiplyVector(initial);
        created.GetComponent<PropNav>().Target = target;
        created.GetComponent<ProximityExplodeScript>().hostileTag = "Enemy";
        created.GetComponent<SupportedMissileScript>().parent = this.gameObject;

    }
}
