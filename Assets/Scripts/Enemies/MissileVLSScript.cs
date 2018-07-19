using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MissileVLSScript : MonoBehaviour {

    private PNSpawner spawner;
    public List<string> targetTags;
    public float lockCone = 90;
    public float lockTime = 3;
    public float fireTime = 20;
    public bool autoFire = false;

    private float fireTimer;
    private GameObject target;

    public bool readyFire { get { return fireTimer < 0; } }

    // Use this for initialization
    void Start () {
        fireTimer = fireTime;
        spawner = GetComponent<PNSpawner>();
	}
	
	// Update is called once per frame
	void Update () {
		if(autoFire)
        {
            if(target == null || Vector3.Angle(target.transform.position - transform.position,transform.forward) > lockCone)
                target = selectClosestInArc();
            if(target != null)
            {
                FireOn(target);
            }
        }
        fireTimer -= Time.deltaTime;
    }

    GameObject selectClosestInArc()
    {
        List<GameObject> selectionList = new List<GameObject>();
        foreach (string tag in targetTags)
        {
            selectionList.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }
        selectionList.RemoveAll(g => Vector3.Angle((g.transform.position - transform.position), transform.forward) > lockCone);
        if (selectionList.Count == 0)
            return null;
        else
            return selectionList.OrderBy(g => (transform.position - g.transform.position).sqrMagnitude).First();
    }

    public void FireOn(GameObject target)
    {
        if (fireTimer < 0)
        {
            fireTimer = fireTime;
            spawner.Create(target);
        }
    }
}
