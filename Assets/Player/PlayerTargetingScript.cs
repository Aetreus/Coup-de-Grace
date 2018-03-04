using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTargetingScript : MonoBehaviour {
    GameObject[] enemies;

    List<GameObject> targetIcons;

    GameObject lockIcon;

    public string targetingTag = "Enemy";

    public GameObject targetPrefab;

    public GameObject lockPrefab;

    GameObject _target;

    GameObject canvas;

    public float getTargetAngle()
    {
        return Vector3.Angle(transform.forward, _target.transform.position);
    }

    public GameObject target {get{return _target;}}

	// Use this for initialization
	void Start () {
        enemies = GameObject.FindGameObjectsWithTag(targetingTag);
        canvas = GameObject.Find("Canvas");
        targetIcons = new List<GameObject>();
        lockIcon = Instantiate(lockPrefab,canvas.transform);
        for(int i = 0; i < enemies.Length; i++)
        {
            targetIcons.Add(Instantiate(targetPrefab,canvas.transform));
        }
        if (enemies.Length != 0)
        {
            _target = enemies[0];
        }
        else
        {
            lockIcon.GetComponent<Image>().enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        enemies = GameObject.FindGameObjectsWithTag(targetingTag);
        int usedIcons = 0;
        while(enemies.Length > targetIcons.Count)
        {
            targetIcons.Add(Instantiate(targetPrefab, canvas.transform));
        }
        foreach (GameObject enemy in enemies)   
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
            
            if (_target.Equals(enemy))
            {
                if (screenPos.z > 1)
                {
                    RectTransform location = lockIcon.GetComponent<RectTransform>();
                    location.anchoredPosition = new Vector2(screenPos.x, screenPos.y) - canvas.GetComponent<RectTransform>().sizeDelta / 2f;
                    lockIcon.GetComponent<Image>().enabled = true;
                }
            }
            else
            {
                RectTransform location = targetIcons[usedIcons].GetComponent<RectTransform>();
                if (screenPos.z > 1)
                {
                    location.anchoredPosition = new Vector2(screenPos.x, screenPos.y) - canvas.GetComponent<RectTransform>().sizeDelta / 2f;
                    targetIcons[usedIcons].GetComponent<Image>().enabled = true;
                    usedIcons++;
                }
            }
        }
        while(usedIcons < targetIcons.Count)
        {
            targetIcons[usedIcons].GetComponent<Image>().enabled = false;
            usedIcons++;
        }
	}

    public void TargetNext()
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            if(enemies.Equals(target))
            {
                if (i + 1 >= enemies.Length)
                {
                    _target = enemies[i];
                }
                else
                    _target = enemies[i + 1];
                break;
            }
        }
    }

    public void TargetCenter()
    {
        float closestDist = Mathf.Infinity;
        foreach(GameObject enemy in enemies)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
            float distFromCenter = Vector3.Distance(new Vector3(screenPos.x, screenPos.y, 0), new Vector3(Screen.width / 2, Screen.height / 2, 0));

            if (distFromCenter <= closestDist)
            {
                closestDist = distFromCenter;
                _target = enemy;
            }
        }
    }
}
