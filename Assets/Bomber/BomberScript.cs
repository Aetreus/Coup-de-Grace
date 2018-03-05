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
    public float bombing_radius;
    public float bomb_spawn_offset;
    private float drop_timer;

    public float missile_fire_angle;
    public float missile_chance;
    public float missile_spawn_offset;
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

        MissileChance();
    }

    void MoveUpdate()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    void CheckBombDrop()
    {
        //float altitude = Mathf.Abs(bomb_target.transform.position.y - transform.position.y);

        //float fallTime = Mathf.Sqrt(altitude / Physics.gravity.magnitude);

        //float horiz_dist = (new Vector3(velocity.x, 0, velocity.z)).magnitude * fallTime;

        float horiz_dist = ((new Vector3(transform.position.x, 0, transform.position.z)) - 
                            (new Vector3(bomb_target.transform.position.x, 0, bomb_target.transform.position.z))).magnitude;

        if (horiz_dist <= bombing_radius)
        {
            DropBomb();

            drop_timer = drop_frequency;
        }
    }

    void DropBomb()
    {
        Instantiate(bomb, transform.position + new Vector3(0, -bomb_spawn_offset, 0), Quaternion.identity);
    }

    void MissileChance()
    {
        Vector3 towardPlayer = player.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, towardPlayer);
        if (angle <= missile_fire_angle && Random.Range(0.0f, 100.0f) <= missile_chance * Time.deltaTime)
        {
            Vector3 spawnLoc = transform.position + towardPlayer.normalized * missile_spawn_offset;
            Quaternion spawnRot = Quaternion.LookRotation(towardPlayer);

            GameObject m = Instantiate(missile, spawnLoc, spawnRot);
            m.GetComponent<PropNav>().Target = player;
        }
    }
}
