﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinimapScript : MonoBehaviour {

    private List<MinimapObject> actives;
    private List<RectTransform> icons;
    private RectTransform rt;

    private int idCount = 0;

    public MinimapObject centerObject;

    public float size = 125;

    public float scale = 80;
    public float maxDynamicScale = 80;
    public float minDynamicScale = 10;
    public float edgeBarrier = 10;

    public bool rotateByCenter;
    public bool scaleDynamic = false;
    public bool isAwake = false;

    private void Awake()
    {
        actives = new List<MinimapObject>();
        icons = new List<RectTransform>();
        rt = GetComponent<RectTransform>();
        isAwake = true;
    }

    // Use this for initialization
    void Start () {
	}

    public GameObject Register(MinimapObject reg)
    {
        actives.Add(reg);
        GameObject temp = Instantiate(reg.Icon, transform);
        icons.Add(temp.GetComponent<RectTransform>());
        temp.name = reg.Icon.name + idCount;
        idCount++;
        return temp;
    }

    public void Unregister(MinimapObject unreg)
    {
        actives.Remove(unreg);
        int index = icons.FindIndex(x => x.name == unreg.Icon.name);
        GameObject.Destroy(icons[index].gameObject, 0.1F);
        icons.RemoveAt(index);
        
    }
	
	// Update is called once per frame
	void Update () {
        foreach(RectTransform r in icons)
        {
            r.gameObject.SetActive(false);
        }

        if(scaleDynamic)
        {
            float maxDist = 0;
            foreach(MinimapObject obj in actives)
            {
                Vector3 relativePos = obj.transform.position - centerObject.transform.position;
                if (maxDist < relativePos.magnitude)
                    maxDist = relativePos.magnitude;
            }
            scale = maxDist / (size - edgeBarrier);
            scale = Math.Max(minDynamicScale,Math.Min(maxDynamicScale, scale));
        }

		foreach(MinimapObject obj in actives)
        {
            RectTransform r = icons.Find(x => x.gameObject.name == obj.Icon.name && !x.gameObject.activeInHierarchy);
            Vector3 relativePos = obj.transform.position - centerObject.transform.position;
            r.localPosition = new Vector3(relativePos.x / scale, relativePos.z / scale);
            r.localRotation = Quaternion.Euler(new Vector3(0, 0, -obj.transform.rotation.eulerAngles.y));
            if(Math.Abs(r.localPosition.x) < rt.sizeDelta.x / 2 && Math.Abs(r.localPosition.y) < rt.sizeDelta.y / 2)
                r.gameObject.SetActive(true);
        }
	}
}
