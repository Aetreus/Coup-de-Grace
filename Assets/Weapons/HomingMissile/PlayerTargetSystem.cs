using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTargetSystem : MonoBehaviour {

    public float lockAngle;
    public float lockTime;

    public float flashTime;

    public string hostileTag;
    
    private float lockTimer;
    private float flashTimer;
    private bool locked;
    private bool flash;
    private bool locking;
    
    private GameObject _target;
    private List<GameObject> enemies;


    private List<GameObject> targetIcons;

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

        canvas = GameObject.Find("Canvas");
        targetIcons = new List<GameObject>();
        lockIcon = Instantiate(lockPrefab, canvas.transform);
        lockIcon.SetActive(false);
        lockSprite = lockIcon.GetComponent<Image>().sprite;
        targetSprite = targetPrefab.GetComponent<Image>().sprite;
        for (int i = 0; i < enemies.Count; i++)
        {
            targetIcons.Add(Instantiate(targetPrefab, canvas.transform));
        }
    }

    // Update is called once per frame
    void Update ()
    {
        GameObject[] e = GameObject.FindGameObjectsWithTag(hostileTag);
        enemies = new List<GameObject>(e);
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

    private void ShowLockIcon()
    {

    }

    private void DisplayIcons()
    {
        int usedIcons = 0;
        while (enemies.Count > targetIcons.Count)
        {
            targetIcons.Add(Instantiate(targetPrefab, canvas.transform));
        }
        foreach (GameObject enemy in enemies)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
            
            if (screenPos.z > 1)
            {
                if (_target != null && _target.Equals(enemy))
                {
                    RectTransform location = lockIcon.GetComponent<RectTransform>();
                    location.anchoredPosition = new Vector2(screenPos.x, screenPos.y) - canvas.GetComponent<RectTransform>().sizeDelta / 2f;
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
                }
                else
                {
                    RectTransform location = targetIcons[usedIcons].GetComponent<RectTransform>();
                    location.anchoredPosition = new Vector2(screenPos.x, screenPos.y) - canvas.GetComponent<RectTransform>().sizeDelta / 2f;
                    targetIcons[usedIcons].GetComponent<Image>().enabled = true;
                    usedIcons++;
                }
            }
        }
        if (_target == null)
            lockIcon.SetActive(false);

        while (usedIcons < targetIcons.Count)
        {
            targetIcons[usedIcons].GetComponent<Image>().enabled = false;
            usedIcons++;
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
