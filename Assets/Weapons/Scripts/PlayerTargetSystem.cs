﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTargetSystem : MonoBehaviour {

    public float lockAngle;
    public float lockTime;

    public float flashTime;

    public string hostileTag;

    public Dictionary<string, GameObject> iconSpec;
    public List<IconSpec> specInit;

    [System.Serializable]
    public class IconSpec
    {
        public string tag;
        public GameObject icon;
    }

    public float referenceDistance;
    public float logBase;
    
    private float lockTimer;
    private float flashTimer;
    private bool locked;
    private bool flash;
    private bool locking;

    private GameObject lockRing;
    
    private GameObject _target;
    private List<GameObject> enemies;


    private Dictionary<string,List<GameObject>> targetIcons;

    private GameObject lockIcon;

    private Sprite lockSprite;
    private Sprite targetSprite;

    public GameObject targetPrefab;

    public GameObject lockPrefab;


    private GameObject canvas;

    // Use this for initialization
    void Start () {
        locked = false;
        locking = false;
        enemies = new List<GameObject>();

        flash = false;
        flashTimer = flashTime;

        lockRing = GameObject.Find("LockRing");
        lockRing.SetActive(false);
        canvas = GameObject.Find("Canvas");
        targetIcons = new Dictionary<string,List<GameObject>>();
        lockIcon = Instantiate(lockPrefab, canvas.transform);
        lockIcon.SetActive(false);
        lockSprite = lockIcon.GetComponent<Image>().sprite;
        targetSprite = targetPrefab.GetComponent<Image>().sprite;

        iconSpec = new Dictionary<string, GameObject>();
        foreach (IconSpec i in specInit)
        {
            iconSpec.Add(i.tag, i.icon);
            targetIcons.Add(i.tag, new List<GameObject>());
        }
        foreach (string tag in iconSpec.Keys)
        {
            GameObject[] e = GameObject.FindGameObjectsWithTag(tag);
            while (targetIcons[tag].Count < e.Length)
                targetIcons[tag].Add(Instantiate(iconSpec[tag], canvas.transform));
            enemies.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }
    }

    // Update is called once per frame
    void Update ()
    {
        enemies = new List<GameObject>();
        foreach (string tag in iconSpec.Keys)
        {
            GameObject[] e = GameObject.FindGameObjectsWithTag(tag);
            while (targetIcons[tag].Count < e.Length)
                targetIcons[tag].Add(Instantiate(iconSpec[tag], canvas.transform));
            enemies.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }
        enemies.Sort(SortByAngle);

        //if you currently have a target you are aiming at or locking onto...
        if (_target != null)
        {
            //if the target was being locked onto long enough, set the lock-on to true
            if (lockTimer <= 0 && locking)
            {
                locked = true;
            }
            //otherwise if locking keep counting down
            else if(lockTimer >= 0 && locking)
            {
                lockTimer -= Time.deltaTime;
            }
            //otherwise...
            else
            {
                //Check if we can start locking onto the target.
                AttemptLock();
            }

            //if the target being aimed at leaves the lock-on angle, lose lock
            Vector3 toTarget = _target.transform.position - transform.position;
            float angleToTarget = Vector3.Angle(toTarget, transform.forward);
            if (angleToTarget > lockAngle)
            {
                locked = false;
                locking = false;
            }
        }
        flashTimer -= Time.deltaTime;
        if(flashTimer <= 0)
        {
            flashTimer = flashTime;
            if (flash)
            {
                flash = false;
            }
            else
                flash = true;
        }
        if (locking == true || locked == true)
        {
            lockRing.SetActive(true);
            ScaleLock(lockAngle);
        }
        else
            lockRing.SetActive(false);
        DisplayIcons();
    }

    public void CycleTarget()
    {
        int targetIndex = enemies.IndexOf(_target);
        targetIndex++;

        if(targetIndex >= enemies.Count)
        {
            targetIndex = 0;
        }

        _target = enemies[targetIndex];

        locked = false;
        locking = false;

        AttemptLock();
    }

    //Attempts to start a lock on the current target.
    private void AttemptLock()
    {
        Vector3 toTarget = _target.transform.position - transform.position;
        float angleToTarget = Vector3.Angle(toTarget, transform.forward);
        if (angleToTarget <= lockAngle)
        {
            lockTimer = lockTime;
            locking = true;
        }
        else
        {
            locking = false;
        }
    }

    public void CenterTarget()
    {
        

        //if there are enemies..
        if (enemies.Count > 0)
        {
            _target = enemies[0];


            locked = false;
            locking = false;

            //if enemy closes to center of view is within the lock-on angle, start locking on to it
            AttemptLock();
        }
    }

    static int SortByAngle(GameObject a, GameObject b)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Vector3 toA = a.transform.position - player.transform.position;
        float angleA = Vector3.Angle(toA, player.transform.forward);

        Vector3 toB = b.transform.position - player.transform.position;
        float angleB = Vector3.Angle(toB, player.transform.forward);

        if(angleA < angleB)
        {
            return -1;
        }
        if (angleA > angleB)
        {
            return 1;
        }
        return 0;
    }

    private void ScaleLock(float angle)
    {
        float size = angle / (Camera.main.fieldOfView / 2) * canvas.GetComponent<RectTransform>().sizeDelta.y;
        lockRing.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
    }

    private void DisplayIcons()
    {
        Dictionary<string,int> usedIcons = new Dictionary<string, int>();
        foreach(string tag in iconSpec.Keys)
        {
            usedIcons[tag] = 0;
        }
        foreach (GameObject enemy in enemies)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
            
            if (screenPos.z > 1)
            {
                if (_target != null && _target.Equals(enemy))
                {
                    RectTransform location = lockIcon.GetComponent<RectTransform>();

                    lockIcon.GetComponent<Image>().color = iconSpec[_target.tag].GetComponent<Image>().color;
                    lockIcon.transform.Find("ObstructedMarker").gameObject.GetComponent<Image>().color = iconSpec[_target.tag].GetComponent<Image>().color;

                    location.anchoredPosition = (new Vector2(screenPos.x, screenPos.y) / canvas.GetComponent<RectTransform>().localScale.x ) - canvas.GetComponent<RectTransform>().sizeDelta / 2f;
                    GameObject DistLabel = lockIcon.transform.Find("DistLabel").gameObject;
                    DistLabel.GetComponent<Text>().text = (transform.position - _target.transform.position).magnitude.ToString();
                    if ((flash && locking) || locked)
                    {
                        lockIcon.SetActive(true);
                        lockIcon.GetComponent<Image>().overrideSprite = null;
                    }
                    else
                    {
                        lockIcon.SetActive(true);
                        lockIcon.GetComponent<Image>().overrideSprite = targetSprite;
                    }
                    
                    //Check if we hit some object other than the target.
                    RaycastHit info;
                    Vector3 raycastSource = transform.position + transform.localToWorldMatrix.MultiplyVector(new Vector3(0, 0, 20));
                    bool hit = Physics.Raycast(transform.position, _target.transform.position - raycastSource, out info,(_target.transform.position - raycastSource).magnitude + 50);
                    if (hit && info.transform.gameObject.tag == "Terrain")
                        lockIcon.transform.Find("ObstructedMarker").gameObject.SetActive(true);
                    else
                        lockIcon.transform.Find("ObstructedMarker").gameObject.SetActive(false);

                    float distance = (transform.position - _target.transform.position).magnitude;
                    
                    //Scale icon by distance
                    if(distance > referenceDistance)
                    {
                        float scaleFactor = 1 / (1 + Mathf.Log(distance / referenceDistance) / Mathf.Log(logBase));
                        lockIcon.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
                    }
                    else
                    {
                        lockIcon.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                else
                {
                    RectTransform location = targetIcons[enemy.tag][usedIcons[enemy.tag]].GetComponent<RectTransform>();
                    location.anchoredPosition = (new Vector2(screenPos.x, screenPos.y) / canvas.GetComponent<RectTransform>().localScale.x) - canvas.GetComponent<RectTransform>().sizeDelta / 2f;
                    targetIcons[enemy.tag][usedIcons[enemy.tag]].SetActive(true);

                    RaycastHit info;
                    bool hit = Physics.Raycast(transform.position, enemy.transform.position - transform.position, out info,(enemy.transform.position - transform.position).magnitude + 50);
                    if (hit && info.transform.gameObject.tag == "Terrain")
                        targetIcons[enemy.tag][usedIcons[enemy.tag]].transform.Find("ObstructedMarker").gameObject.SetActive(true);
                    else
                        targetIcons[enemy.tag][usedIcons[enemy.tag]].transform.Find("ObstructedMarker").gameObject.SetActive(false);


                    float distance = (transform.position - enemy.transform.position).magnitude;

                    //Scale icon by distance
                    if (distance > referenceDistance)
                    {
                        float scaleFactor = 1 / (1 + Mathf.Log(distance / referenceDistance) / Mathf.Log(logBase));
                        targetIcons[enemy.tag][usedIcons[enemy.tag]].transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
                    }
                    else
                    {
                        targetIcons[enemy.tag][usedIcons[enemy.tag]].transform.localScale = new Vector3(1,1, 1);
                    }

                    usedIcons[enemy.tag]++;
                }
            }
        }
        if (_target == null)
            lockIcon.SetActive(false);
        foreach (string tag in iconSpec.Keys)
        {
            while (usedIcons[tag] < targetIcons[tag].Count)
            {
                targetIcons[tag][usedIcons[tag]].SetActive(false);
                usedIcons[tag]++;
            }
        }
    }

    public GameObject Target
    {
        get
        {
            return _target;
        }
    }

    public bool Locked
    {
        get
        {
            return locked;
        }
    }

    public float LockTimer
    {
        get
        {
            return lockTimer;
        }
    }
}
