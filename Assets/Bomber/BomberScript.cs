using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberScript : MonoBehaviour {

    public GameObject missile;
    public Vector3 travel_direction;
    public float speed;
    
    public GameObject bomb;
    public GameObject bomb_target;

    private Vector3 velocity;
    
    public float drop_frequency;
    public float bomb_drop_offset;
    private float drop_timer;

    public float missile_fire_angle;
    public float missile_chance;
    private GameObject player;

	// Use this for initialization
	void Start () {
        transform.forward = travel_direction.normalized;
        velocity = travel_direction.normalized * speed;

        player = GameObject.FindGameObjectWithTag("player");
    }
	
	// Update is called once per frame
	void Update () {
        MoveUpdate();

        if (drop_timer <= 0 )
        {
            CheckBombDrop();
        }
        else
        {
            drop_timer -= Time.deltaTime;
        }

        float angle = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        if(angle <= missile_fire_angle && Random.Range(0.0f, 100.0f) <= missile_chance * Time.deltaTime)
        {
            FireMissile();
        }
    }

    void MoveUpdate()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    void CheckBombDrop()
    {
        float altitude = Mathf.Abs(bomb_target.transform.position.y - transform.position.y);

        float fallTime = Mathf.Sqrt(altitude / Physics.gravity.magnitude);

        float horiz_dist = (new Vector3(velocity.x, 0, velocity.z)).magnitude * fallTime;

        if (horiz_dist <= bomb_drop_offset)
        {
            DropBomb();

            drop_timer = drop_frequency;
        }
    }

    void DropBomb()
    {

    }

    void FireMissile()
    {

    }
}
