using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportedMissileScript : PropNav {

    public float lockCone = 30;
    
    public GameObject parent;

	// Use this for initialization
	protected override void Start () {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        /*if (parent == null && this.transform.parent.gameObject != null)
        {
            parent = this.transform.parent.gameObject;
        }*/
        if (parent != null && Target != null)
        {
            float angle = Vector3.Angle(parent.transform.forward, Target.transform.position - parent.transform.position);
            if (angle > lockCone)
            {
                Target = null;
            }
        }
        base.Update();
    }
}
