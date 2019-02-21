using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMSpawner : PNSpawner {


    public override GameObject Create(GameObject target)
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
        created.GetComponent<PropNav>().target = target;
        created.GetComponent<ProximityExplodeScript>().hostileTag = target.tag;
        created.GetComponent<SupportedMissileScript>().parent = this.gameObject;
        return created;
    }
}
