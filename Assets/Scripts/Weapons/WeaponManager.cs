using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour {
    [System.Serializable]
    public class OnFire : UnityEvent<GameObject> { }
    [SerializeField]
    public OnFire onFire = new OnFire();
    
    public int maximumShots { get {return _maximumShots;} set { _maximumShots = value; ResetLoading(); }}

    public float reloadTime = 2.0F;

    private float current_shots;

    private List<float> loadingTime;

    private PNSpawner sp;

    public float shots { get { return current_shots; } }

    [SerializeField]
    private int _maximumShots = 2;

	// Use this for initialization
	void Start () {
        sp = GetComponent<PNSpawner>();

        loadingTime = new List<float>();

		for(int i = 0; i < maximumShots; i++)
        {
            loadingTime.Add(0.0F);
        }

        current_shots = maximumShots;
	}

    void ResetLoading()
    {
        loadingTime = new List<float>();
        for (int i = 0; i < maximumShots; i++)
        {
            loadingTime.Add(0.0F);
        }
        current_shots = 0;
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

    public void Fire(PlayerTargetSystem pt)
    {
        if (current_shots >= 1 && pt.Locked && pt.Target != null)
        {
            current_shots--;
            for(int i = 0; i< maximumShots; i++)
            {
                if(loadingTime[i] <= 0.0F)
                {
                    loadingTime[i] = reloadTime;
                    sp.Create(pt.Target);
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
