using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : MonoBehaviour {

    private ProjIconScript ms;
    private PropNav pn;
    private GameObject target;

    public GameObject Icon;



    // Use this for initialization
    void Start()
    {
        ms = FindObjectOfType<ProjIconScript>();
        if(gameObject.GetComponent<PropNav>() != null)
        {
            pn = gameObject.GetComponent<PropNav>();
            target = pn.Target;
            if (ms.sourceObject.Equals(target))
                ms.Register(this);
        }
        else
            ms.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(pn.Target != target)
        {
            if(ms.sourceObject.Equals(pn.Target))
            {
                ms.Register(this);
            }
            else if(ms.sourceObject.Equals(target))
            {
                ms.Unregister(this);
            }
            target = pn.Target;
        }
    }

    private void OnDestroy()
    {
        ms.Unregister(this);
    }
}
