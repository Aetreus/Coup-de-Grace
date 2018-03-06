using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropNav : MonoBehaviour {

    public float N = 3;
    public float ref_accel;

    public float damage;
    public string hostileTag;
    public float fueltime;

    private GameObject target = null;
    private Vector3 last_pos;
    private Vector3 target_last_pos;
    
    private FlightBehavior fb;

    // Use this for initialization
    void Start () {
        last_pos = transform.position;
        fb = GetComponent<FlightBehavior>();
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 latex;

        if (target && fueltime > 0)
        {
            Vector3 range_new = target.transform.position - transform.position;
            Vector3 range_old = target_last_pos - last_pos;
            range_new.Normalize();
            range_old.Normalize();

            Vector3 LOS_Delta;
            float LOS_Rate;
            if (range_old.magnitude == 0)
            {
                LOS_Delta = range_new - range_old;
                LOS_Rate = LOS_Delta.magnitude;
            }
            else
            {
                LOS_Delta = range_new - range_old;
                LOS_Rate = LOS_Delta.magnitude;
            }

            float closing_vel = -LOS_Rate;

            float Nt = Physics.gravity.magnitude;
            latex = range_new * N * closing_vel * LOS_Rate + LOS_Delta * Nt * N * 0.5f;

            target_last_pos = target.transform.position;
            last_pos = transform.position;
        }
        else
        {
            latex = Vector3.zero;
        }

        Vector3 local_accel = transform.InverseTransformVector(latex);

        fb.throttle = local_accel.z / ref_accel;
        fb.rudder = local_accel.x / ref_accel;
        fb.elevator = local_accel.y / ref_accel;

        fueltime -= Time.deltaTime;
    }

    public GameObject Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
            target_last_pos = target.transform.position;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(hostileTag))
        {
            HPManager hp = collision.gameObject.GetComponent<HPManager>();
            hp.Damage(damage);
        }

        Destroy(gameObject);
    }
}
