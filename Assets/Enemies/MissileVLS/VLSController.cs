using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class VLSController : MonoBehaviour {
    
    public List<string> targetTags;
    public float lockCone = 90;
    public float lockTime = 3;
    public bool autoFire = true;
    public List<GameObject> vlsLaunchers;
    public float retargetTime;

    private float fireTimer;
    private GameObject target;
    public float retargetTimer;

    // Use this for initialization
    void Start()
    {
        retargetTimer = retargetTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (autoFire)
        {
            if (target == null)
                target = selectClosestInArc();
            else if (Vector3.Angle(target.transform.position - transform.position, transform.forward) > lockCone && retargetTimer < 0)
            {
                retargetTimer = retargetTime;
                target = selectClosestInArc();
            }
            else retargetTimer -= Time.deltaTime;

            if (target != null && Vector3.Angle(target.transform.position - transform.position, transform.forward) < lockCone)
            {
                fireTimer -= Time.deltaTime;
                FireOn(target);
            }
        }
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
        GameObject launcher = vlsLaunchers.Find(l => l.GetComponent<MissileVLSScript>().readyFire);
        if(launcher != null)
        {
            launcher.GetComponent<MissileVLSScript>().FireOn(target);
        }
    }
}
