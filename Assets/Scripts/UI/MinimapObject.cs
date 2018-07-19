using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapObject : MonoBehaviour {


    private MinimapScript ms;
    private bool isAdded = false;

    private RectTransform instantiatedIcon;

    public GameObject Icon;

    void Awake()
    {
        //tryAdd();
    }
        
	// Use this for initialization
	void Start () {
        tryAdd();
	}

    public void tryAdd()
    {
        if (!isAdded)
        {
            ms = FindObjectOfType<MinimapScript>();
            if (ms.isAwake)
            {
                Icon = ms.Register(this);
                isAdded = true;
            }
        }
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
