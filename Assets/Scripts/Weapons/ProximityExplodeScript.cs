﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityExplodeScript : MonoBehaviour {

    private List<GameObject> nearby;

    private GameObject trail;

    public string hostileTag;

    public float triggerRange = 5;

    public float damage = 3;

    public float minDamage = 1;

    public float damageRadius = 10;

    public float innerDamageRadius = 2.5F;

    private bool live = false;

    private float prevDistance = 0.0F;

	// Use this for initialization
	void Start () {
        nearby = new List<GameObject>();
        SphereCollider sc = gameObject.AddComponent<SphereCollider>() as SphereCollider;
        sc.isTrigger = true;
        sc.radius = damageRadius / transform.localScale.x;
        if (transform.Find("Trail") != null)
            trail = transform.Find("Trail").gameObject;
        else
            trail = null;
	}
	
	// Update is called once per frame
	void Update () {
        

        List<float> distances = new List<float>();
        nearby.RemoveAll(g => g == null);
        foreach(GameObject g in nearby)
        {
            if (g.tag == hostileTag)
            {
                distances.Add((g.transform.position - transform.position).magnitude);
            }
        }
        distances.Sort();

        if (live && distances.Count > 0 && distances[0] > prevDistance)
            Explode();

        if (distances.Count > 0 && distances[0] < triggerRange)
        {
            live = true;
            prevDistance = distances[0];
        }
        
	}

    void Explode()
    {
        ExplosionScript es;
        if ((es = GetComponent<ExplosionScript>()) != null)
        {
            es.explode();
        }
        foreach (GameObject g in nearby)
        {
            if (g == null)
                continue;
            HPManager man = g.GetComponent<HPManager>();
            if(man != null)
            {
                float dist = (g.transform.position - gameObject.transform.position).magnitude;
                if(dist < innerDamageRadius)
                {
                    man.Damage(damage);
                }
                else
                {
                    man.Damage((dist - innerDamageRadius) / (damageRadius - innerDamageRadius) * (damage - minDamage) + minDamage);
                }
            }
        }
        //Detach the trail and set it to autodestroy.
        if(trail != null)
        {
            trail.transform.parent = null;
            Destroy(trail, 7);
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        nearby.Add(other.gameObject);
        if (other.tag == hostileTag)
        {
            if(((other.gameObject.transform.position - transform.position).magnitude) < triggerRange)
                Explode();
        }
    }

    void OnTriggerExit(Collider other)
    {
        nearby.Remove(other.gameObject);
    }
}
