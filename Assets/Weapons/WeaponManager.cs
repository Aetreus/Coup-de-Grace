using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour {

    public UnityEvent onFire;

    public int maximumShots = 2;

    public float reloadTime = 2.0F;

    private float current_shots;

    private List<float> loadingTime;

    public float shots { get { return current_shots; } }

	// Use this for initialization
	void Start () {
        if (onFire == null)
            onFire = new UnityEvent();

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
        if (current_shots >= 1)
        {
            current_shots--;
            for(int i = 0; i< maximumShots; i++)
            {
                if(loadingTime[i] <= 0.0F)
                {
                    loadingTime[i] = reloadTime;
                    onFire.Invoke();
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
