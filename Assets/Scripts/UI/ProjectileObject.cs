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
            target = pn.target;
            if (ms.sourceObject.Equals(target))
                ms.Register(this);
        }
        else
            ms.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(pn.target != target)
        {
            if(ms.sourceObject.Equals(pn.target))
            {
                ms.Register(this);
            }
            else if(ms.sourceObject.Equals(target))
            {
                ms.Unregister(this);
            }
            target = pn.target;
        }
    }

    private void OnDestroy()
    {
        ms.Unregister(this);
    }
}
