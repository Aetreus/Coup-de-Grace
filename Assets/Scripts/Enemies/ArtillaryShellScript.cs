using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtillaryShellScript : MonoBehaviour {
    
    public float lifetime;
    public float speed;

    public float damage;
    public string hostileTag;

    private Vector3 velocity;

    // Use this for initialization
    void Start()
    {
        velocity = transform.forward.normalized * speed;
        GetComponent<Rigidbody>().velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(velocity * speed * Time.deltaTime);

        //print(velocity);

        //Decrement remaining lifetime.
        lifetime -= Time.deltaTime;
        if (lifetime < 0)
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
        if (collision.gameObject.tag.Equals(hostileTag))
        {
            HPManager hp = collision.gameObject.GetComponent<HPManager>();
            hp.Damage(damage);
        }

        if (collision.gameObject.tag.Equals("Terrain"))
        {
            Destroy(gameObject);
        }
    }
}
