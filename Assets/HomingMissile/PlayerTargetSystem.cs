using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetSystem : MonoBehaviour {

    public float lock_angle;
    public float lock_time;

    public string hostileTag;
    
    private float timer;
    private bool locked;
    
    private GameObject target;
    private List<GameObject> enemies;

	// Use this for initialization
	void Start () {
        locked = false;
        enemies = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {

        //if you currently have a target you are aiming at or locking onto...
        if (target)
        {
            //if the target was being locked onto long enough, set the lock-on to true
            if (timer <= 0)
            {
                locked = true;
            }
            //otherwise keep counting down
            else
            {
                timer -= Time.deltaTime;
            }

            //if the target being aimed at leaves the lock-on angle, lose the target
            Vector3 toTarget = target.transform.position - transform.position;
            float angleToTarget = Vector3.Angle(toTarget, transform.forward);
            if (angleToTarget > lock_angle)
            {
                target = null;
                locked = false;
            }
        }
        //otherwise...
        else
        {
            //lock on to the center-most target if its in the view angle
            CenterTarget();
        }
    }

    public void CycleTarget()
    {
        enemies.Sort(SortByAngle);
        int targetIndex = enemies.IndexOf(target);
        targetIndex++;

        if(targetIndex >= enemies.Count)
        {
            CenterTarget();
            return;
        }

        Vector3 toTarget = enemies[targetIndex].transform.position - transform.position;
        float angleToTarget = Vector3.Angle(toTarget, transform.forward);
        if (angleToTarget <= lock_angle)
        {
            target = enemies[targetIndex];
            timer = lock_time;
        }
        else
        {
            CenterTarget();
        }
    }

    public void CenterTarget()
    {
        //get all enemies and sort them by smallest angle b/w LOS and forward
        GameObject[] e = GameObject.FindGameObjectsWithTag(hostileTag);
        enemies = new List<GameObject>(e);
        enemies.Sort(SortByAngle);

        //if there are enemies..
        if (enemies.Count > 0)
        {
            //if enemy closes to center of view is within the lock-on angle, start locking on to it
            Vector3 toTarget = target.transform.position - transform.position;
            float angleToTarget = Vector3.Angle(toTarget, transform.forward);
            if (angleToTarget <= lock_angle)
            {
                target = enemies[0];
                timer = lock_time;
            }
        }
    }

    static int SortByAngle(GameObject a, GameObject b)
    {
        GameObject player = GameObject.FindGameObjectWithTag("player");

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

    public GameObject Target
    {
        get
        {
            return target;
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
            return timer;
        }
    }
}
