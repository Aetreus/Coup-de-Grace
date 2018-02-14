using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAim : MonoBehaviour {

    public GameObject target;
    public GameObject bullet;
    public float shotSpeed;
    public float rotateDelta;
    public float minFireAngle;
    public float minFireDist;
    public float fireCooldown;
    public float bulletSpawnOffset;

    private Vector3 targetVel;

    private Vector3 faceVector;
    private float timer = 0;

    // Use this for initialization
    void Start () {
        faceVector = transform.forward.normalized;
    }
	
	// Update is called once per frame
	void Update () {

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
            if (dist <= minFireDist && angle <= minFireAngle)
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
        targetVel = new Vector3(2, 2, 2);
    }

    //
    void UpdateTransform()
    {
        transform.forward = faceVector.normalized;
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

    void Fire()
    {
        Vector3 spawnLoc = transform.position + faceVector.normalized * bulletSpawnOffset;
        Quaternion spawnRot = Quaternion.LookRotation(faceVector);
        Instantiate(bullet, spawnLoc, spawnRot);
        //bullet.GetComponent<AntiAirBulletScript>().SetVel(faceVector.normalized * shotSpeed);
        bullet.GetComponent<AntiAirBulletScript>().speed = shotSpeed;
    }
}
