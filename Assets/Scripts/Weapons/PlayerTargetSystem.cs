using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTargetSystem : MonoBehaviour {

    public float lockAngle;
    public float lockTime;

    public float flashTime;

    public string hostileTag;

    public string lockRingName = "LockRing";
    public string cameraCarrierName = "CameraCarrier";

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
    private GameObject cameraCarrier;
    
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
        cameraCarrier = transform.Find("CameraCarrier").gameObject;
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
        //TODO: This call is expensive, evaluate its utility.
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

        //Show the lock angle ring if we have a target within it.
        if (locking == true || locked == true)
        {
            lockRing.SetActive(true);
            ScaleLock(lockAngle);
        }
        else
            lockRing.SetActive(false);
        DisplayIcons();
    }

    //Select the next closest target
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
        //Is the target within lock angle degrees of the direction the player is facing
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

    //Target the enemy closest to the center of the view.
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

    int SortByAngle(GameObject a, GameObject b)
    {
        //GameObject player = GameObject.FindGameObjectWithTag("Player");

        Vector3 toA = a.transform.position - cameraCarrier.transform.position;

        //Ref:https://stackoverflow.com/questions/16542042/fastest-way-to-sort-vectors-by-angle-without-actually-computing-that-angle
        Vector3 normProjA = Vector3.Project(toA, cameraCarrier.transform.forward);
        Vector3 perpinA = toA - normProjA;
        float distNormA = normProjA.magnitude;
        float perpinDistA = perpinA.magnitude;
        float angleA = distNormA / (distNormA + Mathf.Abs(perpinA.magnitude));
        if (perpinDistA < 0)
            angleA += 3;
        else
            angleA = 1 - angleA;
            

        //float angleA = Vector3.Angle(toA, cameraCarrier.transform.forward);

        Vector3 toB = b.transform.position - cameraCarrier.transform.position;

        
        Vector3 normProjB = Vector3.Project(toB, cameraCarrier.transform.forward);
        Vector3 perpinB = toB - normProjB;
        float distNormB = normProjB.magnitude;
        float perpinDistB = perpinB.magnitude;
        float angleB = distNormB / (distNormB + Mathf.Abs(perpinB.magnitude));
        if (perpinDistB < 0)
            angleB += 3;
        else
            angleB = 1 - angleB;
            

        //float angleB = Vector3.Angle(toB, cameraCarrier.transform.forward);

        if (angleA < angleB)
        {
            return -1;
        }
        if (angleA > angleB)
        {
            return 1;
        }
        return 0;
    }

    //Scale the lock ring to the correct size for the current lock angle
    private void ScaleLock(float angle)
    {
        CameraControlScript cs = cameraCarrier.GetComponent<CameraControlScript>();
        float test = Camera.main.fieldOfView;
        test = canvas.GetComponent<RectTransform>().sizeDelta.y;
        float xPos = cameraCarrier.GetComponent<CameraControlScript>().Azimuth / (Camera.main.fieldOfView) * canvas.GetComponent<RectTransform>().sizeDelta.y;
        float yPos = cameraCarrier.GetComponent<CameraControlScript>().Altitude / (Camera.main.fieldOfView) * canvas.GetComponent<RectTransform>().sizeDelta.y;
        float size = angle / (Camera.main.fieldOfView / 2) * canvas.GetComponent<RectTransform>().sizeDelta.y;
        lockRing.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
        lockRing.GetComponent<RectTransform>().localPosition = new Vector3(xPos, yPos);
    }

    //Display the correct icons over each enemy
    private void DisplayIcons()
    {
        Dictionary<string,int> usedIcons = new Dictionary<string, int>();
        foreach(string tag in iconSpec.Keys)
        {
            usedIcons[tag] = 0;
        }

        foreach (GameObject enemy in enemies)
        {
            //Get the enemies screen position.
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
            
            //If the enemy is in front of the player.
            if (screenPos.z > 1)
            {
                //If we have a target, show them with a distance marker.
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
                    //Other targets get a standard icon.
                    RectTransform location = targetIcons[enemy.tag][usedIcons[enemy.tag]].GetComponent<RectTransform>();
                    location.anchoredPosition = (new Vector2(screenPos.x, screenPos.y) / canvas.GetComponent<RectTransform>().localScale.x) - canvas.GetComponent<RectTransform>().sizeDelta / 2f;
                    targetIcons[enemy.tag][usedIcons[enemy.tag]].SetActive(true);

                    //Check if we have an obstruction
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
