using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurretScript : TurretAim {

    private PNSpawner spawner;
    public float aimShotSpeed;
    public float lockTime;

	// Use this for initialization
	protected override void Start () {

        faceVector = transform.forward.normalized;

        shotSpeed = aimShotSpeed;
        spawner = gameObject.AddComponent<PNSpawner>();
        spawner.spawned = bullet;
        spawner.offset = new Vector3(0,0,bulletSpawnOffset);
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        float angle = Vector3.Angle(faceVector, target.transform.position - gameObject.transform.position);
        if (angle > maxFireAngle)
        {
            timer = lockTime;
        }
	}

    protected override void Fire()
    {
        spawner.Create(target);        
    }
}
