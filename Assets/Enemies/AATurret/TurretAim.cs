using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretAim : MonoBehaviour {

    public GameObject target;
    public List<string> targetTags;
    public GameObject bullet;
    public float rotateDelta;
    public float maxFireAngle;
    public float maxFireDist;
    public float fireCooldown;
    public float bulletSpawnOffset;
    public string playerTag;
    
    protected float shotSpeed;

    private Vector3 targetVel;

    protected Vector3 faceVector;
    protected float timer = 0;

    // Use this for initialization
    protected virtual void Start () {
        faceVector = transform.forward.normalized;

        shotSpeed = bullet.GetComponent<ArtillaryShellScript>().speed;
    }
	
	// Update is called once per frame
	protected virtual void Update () {

        if (!target)
        {
            SelectTarget();
            if (!target)
            return;
        }
        //get the velocity of the target
        UpdateVelocity();
        
        //calculate the vector that a shot must be fired in to hit the target
        Vector3 intercept = InterceptVector();

        //rotate the faceVector toward
        faceVector = Vector3.RotateTowards(faceVector, intercept, rotateDelta*Time.deltaTime, 0.0f).normalized;

        //change the turret's position to match the faceVector
        UpdateTransform();

        //if a bullet is available...
        if (timer <= 0)
        {
            float dist = Vector3.Distance(target.transform.position, transform.position);
            float angle = Vector3.Angle(faceVector, intercept);

            //if the target is close enough and the faceVector is close enough to the intercept
            if (dist <= maxFireDist && angle <= maxFireAngle)
            {
                //fire and reset the cooldown
                Fire();
                timer = fireCooldown;
            }
        }
        else
        {
            timer -= Time.deltaTime;
        }
	}

    //get the target's velocity information
    void UpdateVelocity()
    {
        targetVel = target.GetComponent<Rigidbody>().velocity;
    }

    //
    void UpdateTransform()
    {
        //transform.forward = faceVector.normalized;

        transform.right = Vector3.Normalize(new Vector3(faceVector.x, 0, faceVector.z));

        float zAngle = Vector3.Angle(faceVector, transform.right);

        if((faceVector+transform.position).y < transform.position.y)
        {
            zAngle *= -1;
        }

        Transform pivot = transform.GetChild(0);

        pivot.localEulerAngles = new Vector3(0, 0, zAngle);

        //pivot.right = Vector3.Normalize(new Vector3(faceVector.x, faceVector.y, 0));

        //float angle = Vector3.Angle(Vector3.right, new Vector3(0, 0, faceVector.z));

        //pivot.localRotation = new Quaternion();

        //pivot.right = new Vector3(0, 0, faceVector.y);
    }

    //method found here: http://danikgames.com/blog/how-to-intersect-a-moving-target-in-2d/
    Vector3 InterceptVector()
    {
        //get direction from this object to the target
        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

        //the orthographic component of the target's velocity
        Vector3 targetVelOrth = Vector3.Dot(targetVel, dirToTarget) * dirToTarget;

        //the tangetential component of the target's velocity 
        Vector3 targetVelTang = targetVel - targetVelOrth;

        //the orthographic vel of the shot must be equal to that of the target
        Vector3 shotVelTang = targetVelTang;

        float shotVelSpeed = shotVelTang.magnitude;

        //if the orthographic shot vel is greater than the max shot speed,
        //aim in the target's velocity
        if(shotVelSpeed > shotSpeed)
        {
            return targetVel.normalized * shotSpeed;
        }

        //find the shot's orthographic speed with pythagorian theorem
        float shotSpeedOrth = Mathf.Sqrt(shotSpeed * shotSpeed - shotVelSpeed * shotVelSpeed);
        Vector3 shotVelOrth = dirToTarget * shotSpeedOrth;

        return (shotVelOrth + shotVelTang).normalized;
    }

    protected virtual void Fire()
    {
        Vector3 spawnLoc = transform.position + faceVector.normalized * bulletSpawnOffset;
        Quaternion spawnRot = Quaternion.LookRotation(faceVector);
        Instantiate(bullet, spawnLoc, spawnRot);
        //bullet.GetComponent<AntiAirBulletScript>().SetVel(faceVector.normalized * shotSpeed);
    }

    protected virtual void SelectTarget()
    {
        List<GameObject> selectionList = new List<GameObject>();
        foreach (string tag in targetTags)
        {
            selectionList.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }
        if (selectionList.Count == 0)
            target = null;
        else
            target = selectionList.OrderBy(g => (transform.position - g.transform.position).sqrMagnitude).First();
    }

    public virtual void SeekTarget()
    {

    }
}
