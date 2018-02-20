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
        float newVal = hp + value;
        SetHP(newVal);
    }

    public void Damage(float damage)
    {
        ChangeHP(-damage);
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}
