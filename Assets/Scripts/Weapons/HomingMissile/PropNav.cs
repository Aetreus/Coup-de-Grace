using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (FlightBehavior)),RequireComponent(typeof (Rigidbody))]
public class PropNav : MonoBehaviour {

    public float N = 3;
    public float ref_accel;
    public float ref_speed = 200;

    public float damage;
    public float fueltime;
    public float lifetime;
    public float dampingTime;
    public float viewCone = 40;

    public float targetDistance { get { return _targetDistance; } }

    public PIDController surfaceController;

    private GameObject target = null;
    private Vector3 last_pos;
    private Vector3 target_last_pos;

    private Vector3 last_vel;
    
    private FlightBehavior fb;

    private Rigidbody rb;

    private float _targetDistance;

    private float dampingTimer;

    // Use this for initialization
    protected virtual void Start () {
        last_pos = transform.position;
        fb = GetComponent<FlightBehavior>();
        rb = GetComponent<Rigidbody>();
        last_vel = rb.velocity;
        dampingTimer = dampingTime;
    }
	
	// Update is called once per frame
	protected virtual void Update () {


        Vector3 latax;

        //Check if target in view cone before calculating latax
        if (target)
        {
            float targetAngle = Vector3.Angle(this.transform.forward, target.transform.position - transform.position);
            if (targetAngle > viewCone)
                target = null;
        }

        if (target)
        {

            Vector3 range = target.transform.position - transform.position;

            _targetDistance = range.magnitude;

            Vector3 missile_vel = GetComponent<Rigidbody>().velocity;

            Vector3 relative_vel;
            if (target.GetComponent<Rigidbody>() == null)
            {
                relative_vel = -missile_vel;   
            }
            else
            {
                relative_vel = target.GetComponent<Rigidbody>().velocity - missile_vel;
            }
            
            Vector3 rotation_vec = Vector3.Cross(range, relative_vel) / Vector3.Dot(range, range);

            Vector3 term1 = -N * relative_vel.magnitude * missile_vel.normalized;

            latax = Vector3.Cross(term1, rotation_vec);
        }
        else
        {
            latax = Vector3.zero;
        }

        //Compare previous acceleration to latax

        Vector3 accel = (rb.velocity - last_vel) / Time.deltaTime;

        Vector3 delta_accel = accel - latax;

        Vector3 local_delta = transform.InverseTransformVector(delta_accel);

        //print(local_accel);

        if (lifetime < 0)
        {
            Destroy(gameObject);
        }
        
        //Linearly damp outputs for short period of flight.
        float surfaceDampCoeff = 1.0F;
        if(dampingTimer > 0)
        {
            surfaceDampCoeff = 1.0F - (dampingTime / dampingTimer);
            dampingTime -= Time.deltaTime;
        }
        surfaceDampCoeff = Mathf.Min(surfaceDampCoeff, 1);
            

        if(fueltime > 0)
        {
            fb.throttle = 1.0F;
        }
        else
        {
            fb.throttle = 0.0F;
        }

        //Higher speed accelerations are acheived with square less control inputs.
        float speedControlSense = 1.0F;
        /*if(rb.velocity.sqrMagnitude > ref_speed)
        {
            speedControlSense = ref_speed * ref_speed / rb.velocity.sqrMagnitude;
        }*/



        Debug.DrawLine(transform.position, transform.position + latax);
        Debug.DrawLine(transform.position, transform.position + accel);

        fb.rudder += surfaceController.Calc((local_delta.x / ref_accel)  * surfaceDampCoeff) * speedControlSense;
        fb.elevator += surfaceController.Calc((-local_delta.y / ref_accel) * surfaceDampCoeff) * speedControlSense;



        fueltime -= Time.deltaTime;
        lifetime -= Time.deltaTime;

        last_vel = rb.velocity;
    }

    public GameObject Target
    {
        get
        {
            return target;
        }
        set
        {
            //Add/remove target warnings if we target the player.
            if (target != null)
            {
                PlayerControlBehavior pcb = target.GetComponent<PlayerControlBehavior>();
                if (pcb != null)
                {
                    pcb.warnings.RemoveAll(w => w.reference == gameObject);
                }
            }
            target = value;
            if (target != null)
            {
                target_last_pos = target.transform.position;
                if (target.GetComponent<PlayerControlBehavior>() != null)
                {
                    PlayerControlBehavior pcb = target.GetComponent<PlayerControlBehavior>();
                    PlayerControlBehavior.Warning warn = new PlayerControlBehavior.Warning(gameObject, "PropNav", "targetDistance", false, null, false, 2000, "MISSILE", "None", target.transform.Find("UISoundHolder/MissileAlertPlayer").GetComponent<AudioSource>(), true);
                    pcb.warnings.Add(warn);
                    pcb.UpdateWarnings();
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (target && collision.gameObject == target)
        {
            HPManager hp = collision.gameObject.GetComponent<HPManager>();
            hp.Damage(damage);
        }

        ExplosionScript s = gameObject.GetComponent<ExplosionScript>();
        if (s != null)
            s.explode();

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (target != null)
        {
            PlayerControlBehavior pcb = target.GetComponent<PlayerControlBehavior>();
            if (pcb != null)
            {
                target.transform.Find("UISoundHolder/MissileAlertPlayer").GetComponent<AudioSource>().Stop();
                pcb.warnings.RemoveAll(w => w.reference == gameObject);
            }
        }
    }
}
