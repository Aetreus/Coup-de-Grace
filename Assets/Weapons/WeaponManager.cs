﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent( typeof(PlayerTargetSystem))]
public class WeaponManager : MonoBehaviour {
    [System.Serializable]
    public class OnFire : UnityEvent<GameObject> { }
    [SerializeField]
    public OnFire onFire = new OnFire();

    private PlayerTargetSystem pt;

    public int maximumShots = 2;

    public float reloadTime = 2.0F;

    private float current_shots;

    private List<float> loadingTime;

    public float shots { get { return current_shots; } }

	// Use this for initialization
	void Start () {

        pt = GetComponent<PlayerTargetSystem>();

        loadingTime = new List<float>();

		for(int i = 0; i < maximumShots; i++)
        {
            loadingTime.Add(0.0F);
        }

        current_shots = maximumShots;
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < maximumShots; i++)
        {
            if(loadingTime[i] > 0.0F)
            {
                loadingTime[i] -= Time.deltaTime;
                if (loadingTime[i] <= 0.0F)
                    current_shots++;
            }
        }
	}

    public void Fire()
    {
        if (current_shots >= 1 && pt.Locked && pt.Target != null)
        {
            current_shots--;
            for(int i = 0; i< maximumShots; i++)
            {
                if(loadingTime[i] <= 0.0F)
                {
                    loadingTime[i] = reloadTime;
                    onFire.Invoke(pt.Target);
                    return;
                }
            }
        }
    }

    public float GetLoadingFraction(int index)
    {
        return 1 - loadingTime[index] / reloadTime;
    }
}
