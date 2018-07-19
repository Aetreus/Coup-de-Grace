using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlScript : MonoBehaviour {

    public GameObject player;

    public float manualSlewRate;
    public float autoSlewRate;
    public float slewScaleTime;
    public float altSlewMax;

    private float altitude;
    private float azimuth;
    private float slewTimer;
    private float autoSlewScaleDist;

    public float Altitude { get { return altitude; } }
    public float Azimuth { get { return -azimuth; } }

    private bool centeringView;

	// Use this for initialization
	void Start () {
        altitude = transform.rotation.eulerAngles.x;
        azimuth = transform.rotation.eulerAngles.z;
        slewTimer = 0;
        autoSlewScaleDist = slewScaleTime * autoSlewRate / 2;
        centeringView = true;
	}
	
	// Update is called once per frame
	void Update () {
        float scaledAltInput = Input.GetAxis("ViewAlt");
        float scaledAzInput = Input.GetAxis("ViewAz");

        if (Mathf.Abs(scaledAltInput) > 0 || Mathf.Abs(scaledAzInput) > 0)
            centeringView = false;

        
        if(Input.GetButton("CenterTarget") && player.GetComponent<PlayerTargetSystem>().Target != null)
        {
            slewTimer = 0;
            GameObject target = player.GetComponent<PlayerTargetSystem>().Target;
            Vector3 toTarget = target.transform.position - player.transform.position;
            float targetAlt = Vector3.Angle(player.transform.forward, Vector3.ProjectOnPlane(toTarget, player.transform.right));
            if (Vector3.Dot(Vector3.Cross(toTarget, player.transform.forward), player.transform.right) > 0)//Get the sign of the angle difference
            {
                targetAlt = -targetAlt;
            }
            float targetAz = Vector3.Angle(player.transform.forward, Vector3.ProjectOnPlane(toTarget, player.transform.up));
            if (Vector3.Dot(Vector3.Cross(toTarget, player.transform.forward), player.transform.up) > 0)//Get the sign of the angle difference
            {
                targetAz = -targetAz;
            }
            float altError = targetAlt - altitude;
            float azError = targetAz - azimuth;

            if (Mathf.Abs(altError) > autoSlewRate * Time.deltaTime)
                altitude += Mathf.Sign(altError) * autoSlewRate * Time.deltaTime;
            else
                altitude = targetAlt;
            if (Mathf.Abs(azError) > autoSlewRate * Time.deltaTime)
                azimuth += Mathf.Sign(azError) * autoSlewRate * Time.deltaTime;
            else
                azimuth = targetAz;
            
        }
        else if (centeringView || Input.GetButton("CenterView"))
        {
            slewTimer = 0;
            /*
            if(altitude < autoSlewScaleDist)
            {
                altitude -= Mathf.Sign(altitude) * Mathf.Sqrt(Mathf.Abs(altitude) / autoSlewScaleDist) * autoSlewRate * Time.deltaTime;
            }
            else*/
            centeringView = true;
            if (Mathf.Abs(altitude) > autoSlewRate * Time.deltaTime)
                altitude -= Mathf.Sign(altitude) * autoSlewRate * Time.deltaTime;
            else
                altitude = 0;
            if (Mathf.Abs(azimuth) > autoSlewRate * Time.deltaTime)
                azimuth -= Mathf.Sign(azimuth) * autoSlewRate * Time.deltaTime;
            else
                azimuth = 0;
        }
        else if (Mathf.Abs(scaledAltInput) > 0 || Mathf.Abs(scaledAzInput) > 0)
        {
            if(slewTimer < 1.0)
                slewTimer += Time.deltaTime / slewScaleTime;
            altitude += scaledAltInput * manualSlewRate * Time.deltaTime * slewTimer;
            azimuth += scaledAzInput * manualSlewRate * Time.deltaTime * slewTimer;
        }
        else
        {
            slewTimer = 0;
        }
        if (Mathf.Abs(altitude) > altSlewMax)
            altitude = Mathf.Sign(altitude) * altSlewMax;
        if (Mathf.Abs(azimuth) > 180)
            azimuth = Mathf.Sign(azimuth) * -1 * 180 -  Mathf.Abs(azimuth - Mathf.Sign(azimuth) * 180);

        transform.localRotation = Quaternion.Euler(altitude, azimuth, 0);

    }
}
