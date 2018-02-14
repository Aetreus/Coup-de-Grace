using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAirBulletScript : MonoBehaviour {

    public float speed;
    public float timer;

    private Vector3 velocity;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        //print(speed);

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void SetVel(Vector3 direction)
    {
        print(direction);
        print(velocity);
        velocity = direction.normalized * speed;
        print(velocity);
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
        //insert what to do when hitting player

        GameObject.Destroy(gameObject);
    }
    */
}
