using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMissileAim : MonoBehaviour {

    public GameObject missile;
    public float missileSpeed;
    public float maxAimDistance; //if the object thte crosshair is pointed at is within this distance, the bullet will home at the object
    public float missileStartOffset;
    public float fireCooldown; //the minimum time between firing

    private float fireTimer;

    // Use this for initialization
    void Start () {
        fireTimer = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0) && fireTimer <= 0)
        {
            //Vector3 aimBearing = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;

            Vector3 aimBearing = transform.forward;

            GameObject shot = Instantiate(missile, transform.position + aimBearing * missileStartOffset, Quaternion.identity);

            //if a ray from the camera through the crosshair hits something, set the target to that point.
            //otherwise, set the target to the maxAimDistance in the direction of that ray
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            Vector3 playerToTarget;
            if (Physics.Raycast(ray, out hit, maxAimDistance))
            {
                playerToTarget = (hit.point - shot.transform.position).normalized;
            }
            else
            {
                Vector3 target = ray.direction * maxAimDistance + transform.position;
                playerToTarget = (target - shot.transform.position).normalized;
            }

            //set the velocity of the bullet toward the target at the missileSpeed
            shot.transform.forward = playerToTarget;
            shot.GetComponent<Rigidbody>().velocity = playerToTarget * missileSpeed;

            fireTimer = fireCooldown;
        }
        fireTimer = Mathf.Clamp(fireTimer - Time.deltaTime, 0, fireCooldown);

    }
}
