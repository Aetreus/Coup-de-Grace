using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissileAim : MonoBehaviour {

    public GameObject missile;
    public float missileSpeed;
    public float aimRadius;
    public float fireCooldown;
    public string enemyTag;
    public float missileSpawnOffset;

    private float timer;

	// Use this for initialization
	void Start () {
        timer = 0;
	}

    // Update is called once per frame
    void Update() {

        if (Input.GetButton("Fire1") && timer <= 0)
        {
            CheckAim();
            timer = fireCooldown;
        }
        timer = Mathf.Clamp(timer - Time.deltaTime, 0, fireCooldown);
    }

    void CheckAim()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject middleEnemy = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
            float distFromCenter = Vector3.Distance(new Vector3(screenPos.x, screenPos.y, 0), new Vector3(Screen.width / 2, Screen.height / 2, 0));
            print("Dist from center: " + distFromCenter);

            Vector3 viewPoint = Camera.main.WorldToViewportPoint(enemy.transform.position);
            bool onScreen = viewPoint.z > 0 && viewPoint.x > 0 && viewPoint.x < 1 && viewPoint.y > 0 && viewPoint.y < 1;
            print("On Screen: " + onScreen);

            if (distFromCenter <= aimRadius && onScreen && distFromCenter <= closestDist)
            {
                print("Closest updated");
                closestDist = distFromCenter;
                middleEnemy = enemy;
            }
        }

        Vector3 towardTarget;
        if(closestDist != Mathf.Infinity)
        {
            print("valid target");
            towardTarget = middleEnemy.transform.position - transform.position;
        }
        else
        {
            print("no valid target");
            towardTarget = transform.forward;
        }
        Vector3 spawnLoc = transform.position + towardTarget.normalized * missileSpawnOffset;
        Quaternion spawnRot = Quaternion.LookRotation(towardTarget);
        Instantiate(missile, spawnLoc, spawnRot);
        missile.GetComponent<ArtillaryShellScript>().speed = missileSpeed;
    }
}
