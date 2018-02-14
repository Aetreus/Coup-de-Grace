using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAirBulletScript : MonoBehaviour {
    
    public float timer;
    public float speed;

    private Vector3 velocity = new Vector3(0, 0, 1).normalized;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(velocity * speed * Time.deltaTime);

        //print(velocity);

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
    /*
    public void SetVel(Vector3 v)
    {
        print(velocity);
        print(v);
        velocity = v;
        print(velocity);
    }
    */
    
    void OnCollisionEnter(Collision collision)
    {
        //insert what to do when hitting player

        GameObject.Destroy(gameObject);
    }
}
