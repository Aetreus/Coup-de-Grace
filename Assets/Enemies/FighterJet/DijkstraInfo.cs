﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraInfo : MonoBehaviour {

    private List<GameObject> node_neighbors;

    private List<GameObject> neighbors;
    private float dist;
    private GameObject prev;

    // Use this for initialization
    void Start()
    {
        node_neighbors = Calc_Node_Neighbors();

        Reset(null, null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<GameObject> Neighbors
    {
        get
        {
            return neighbors;
        }
    }

    public float Dist
    {
        set
        {
            dist = value;
        }
        get
        {
            return dist;
        }
    }

    public GameObject Prev
    {
        set
        {
            prev = value;
        }
        get
        {
            return prev;
        }
    }

    //set this nodes neighbors to all of the neighboring nodes plus start and end if you can travel straight to them.
    //also set dist to infinity and prev to unidentified
    public void Reset(GameObject start, GameObject end)
    {
        neighbors = new List<GameObject>();
        neighbors.AddRange(node_neighbors);
        if (start != null && this.gameObject != start)
        {
            if (TargetVisible(start))
            {
                neighbors.Add(start);
            }
        }
        if(end != null && this.gameObject != end)
        {
            if (TargetVisible(end))
            {
                neighbors.Add(end);
            }
        }

        dist = Mathf.Infinity;
        prev = null;
    }

    List<GameObject> Calc_Node_Neighbors()
    {
        List<GameObject> nodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("Node"));
        List<GameObject> neighbors = new List<GameObject>();

        foreach (GameObject node in nodes)
        {
            if(TargetVisible(node))
            {
                neighbors.Add(node);
            }
        }

        return neighbors;
    }

    bool TargetVisible(GameObject target)
    {
        Vector3 towardTarget = target.transform.position - transform.position;

        RaycastHit hitinfo;
        bool hit = Physics.Raycast(transform.position, towardTarget, out hitinfo, towardTarget.magnitude);

        if (hit && (hitinfo.collider.gameObject.tag == "Terrain" || hitinfo.collider.gameObject.tag == "Building"))
        {
            return false;
        }

        return true;
    }
}
