using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HPManager : MonoBehaviour {

    public float maxHP;

    public UnityEvent onDie = new UnityEvent();

    [SerializeField]
    private float hp;

	// Use this for initialization
	void Start () {
        if (onDie.GetPersistentEventCount() == 0)
            onDie.AddListener(delegate { Destroy(gameObject); });
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

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void Die()
    {
        
        onDie.Invoke();
    }
}
