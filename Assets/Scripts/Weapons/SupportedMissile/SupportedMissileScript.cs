using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportedMissileScript : PropNav {

    public float lockCone = 30;
    
    [SerializeField]
    private GameObject _parent;

    public GameObject parent
    {
        get { return _parent; }
        set { _parent = value; if(_parent != null) _sup = _parent.GetComponent<SupportSource>(); }
    }

    private SupportSource _sup;

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
        //Lose target if we have a support source and it indicates the target is outside of its tracking
        if(_sup != null)
        {
            if (target != null && !_sup.HasTrack(target))
                target = null;
        }
        //If we don't have a support source script, track based on our own lock cone and the parent object's facing vector.
        else if (parent != null && target != null)
        {
            float angle = Vector3.Angle(parent.transform.forward, target.transform.position - parent.transform.position);
            if (angle > lockCone)
            {
                target = null;
            }
        }
        base.Update();
    }
}
