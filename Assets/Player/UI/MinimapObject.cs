using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapObject : MonoBehaviour {

    private MinimapScript ms;

    public GameObject Icon;
    
	// Use this for initialization
	void Start () {
        ms = FindObjectOfType<MinimapScript>();
        ms.Register(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        if(ms != null)
            ms.Unregister(this);
    }
}
