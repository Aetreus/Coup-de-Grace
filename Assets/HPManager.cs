using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPManager : MonoBehaviour {

    public float maxHP;

    private float hp;

	// Use this for initialization
	void Start () {
        hp = maxHP;
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SetHP(float value)
    {
        hp = Mathf.Clamp(value, 0, maxHP);

        if (hp <= 0)
        {
            Die();
        }
    }

    public void ChangeHP(float value)
    {
        SetHP(hp + value);
    }

    public void Damage(float damage)
    {
        ChangeHP(-damage);
    }

    public void Die()
    {
        if (gameObject.tag.Equals("Player"))
        {
            transform.position = new Vector3(14, 700, 16000);
            transform.rotation = Quaternion.Euler(Vector3.zero);
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 150);
            gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        }
        else
            Destroy(gameObject);
    }
}
