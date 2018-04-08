using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDecisionTree : MonoBehaviour {

    public float accel_mag;

    //DetectCollision variables
    public float collision_whisker_len;
    public float collision_whisker_offset;
    public float collision_personal_space;

    //PlayerMissileApproaching variables
    public float max_player_missile_approaching_angle;

    //PlayerMissileClose variables
    public float max_player_missile_close_dist;

    //MyMissileCloseToPlayer variable
    public float max_my_missile_close_distance;

    //MyMissileApproachingPlayer variable
    public float max_my_missile_approaching_angle;

    //InPlayerMissileCone variables
    public float player_missile_cone_angle;
    public float player_missile_cone_len;

    //PlayerInMissileCone variables;
    public float fighter_missile_cone_angle;
    public float fighter_missile_cone_len;

    //Sharp_Pitch variables
    public float min_pitch_ground_dist;

    //Move_Behind_Player variable
    public float max_in_front_of_player_angle;
    public float min_far_away_dist;

    //Predict_Player_Loc variables
    //public float max_prediction_time;

    private GameObject activeMissile = null;
    private GameObject player;

    private WeaponManager wm;

    private Vector3 linear_accel;

    private string debug_msg;

    // Use this for initialization
    void Start() {
        linear_accel = transform.forward * accel_mag;
        player = GameObject.FindGameObjectWithTag("Player");
        wm = GetComponent<WeaponManager>();
    }

    // Update is called once per frame
    void Update() {

        debug_msg = "";
        linear_accel = DetectCollisionTree().normalized * accel_mag;
        Debug.Log(debug_msg, this);
    }

    public Vector3 LinearAcceleration
    {
        get
        {
            return linear_accel;
        }
    }

    //forks of the tree------------------------------------------------------

    Vector3 DetectCollisionTree()
    {
        Vector3 avoidVec = DetectCollision();

        if(avoidVec.magnitude > 0)
        {
            debug_msg += "Detect Collision T";
            return avoidVec;
        }
        else
        {
            debug_msg += "Detect Collision F";
            return PlayerMissileApproachingTree();
        }
    }

    Vector3 PlayerMissileApproachingTree()
    {
        GameObject closest_missile = PlayerMissileApproaching();

        if(closest_missile != null)
        {
            debug_msg += " -> PlayerMissileApproaching T";
            return PlayerMissileCloseTree(closest_missile);
        }
        else
        {
            debug_msg += " -> PlayerMissileApproaching F";
            return MyMissileApproachingPlayerTree();
        }
    }

    Vector3 PlayerMissileCloseTree(GameObject closest_missile)
    {
        if(PlayerMissileClose(closest_missile))
        {
            debug_msg += " -> PlayerMissileClose T -> return Sharp Pitch";
            return Sharp_Pitch();
        }
        else
        {
            debug_msg += " -> PlayerMissileClose F";
            return MyMissileCloseToPlayerTree(closest_missile);
        }
    }

    Vector3 MyMissileCloseToPlayerTree(GameObject closest_missile)
    {
        if(MyMissileCloseToPlayer())
        {
            debug_msg += " -> MyMissileCloseToPlayer T -> return Seek Player";
            return Seek_Player();
        }
        else
        {
            debug_msg += " -> MyMissileCloseToPlayer F -> return Turn_Side_Toward_Missile";
            return Turn_Side_Toward_Missile(closest_missile);
        }
    }

    Vector3 MyMissileApproachingPlayerTree()
    {
        if(MyMissileApproachingPlayer())
        {
            debug_msg += " -> MyMissileApproachingPlayer T -> return Seek Player";
            return Seek_Player();
        }
        else
        {
            debug_msg += " -> MyMissileApproachingPlayer F";
            return PlayerInLOSTree();
        }
    }

    Vector3 PlayerInLOSTree()
    {
        if (PlayerInLOS())
        {
            debug_msg += " -> PlayerInLOS T";
            return InPlayerMissileConeTree();
        }
        else
        {
            debug_msg += " -> PlayerInLOS F -> return FollowPathToPlayer";
            return FollowPathToPlayer();
        }
    }

    Vector3 InPlayerMissileConeTree()
    {
        if(InPlayerMissileCone())
        {
            debug_msg += " -> InPlayerMissileCone T -> return Exit_Missile_Cone";
            return Exit_Missile_Cone();
        }
        else
        {
            debug_msg += " -> InPlayerMissileCone F";
            return MissileAvailableTree();
        }
    }

    Vector3 MissileAvailableTree()
    {
        if(MissileAvailable())
        {
            debug_msg += " -> MissileAvailable T";
            return PlayerInMissileConeTree();
        }
        else
        {
            debug_msg += " -> InPlayerMissileCone F -> return Move_Behind_Player";
            return Move_Behind_Player();
        }
    }

    Vector3 PlayerInMissileConeTree()
    {
        if(PlayerInMissileCone())
        {
            debug_msg += " -> PlayerInMissileCone T -> Fire Missile";
            //fire missile
        }
        else
        {
            debug_msg += " -> PlayerInMissileCone F";
        }

        debug_msg += "-> return Seek_Player";
        return Seek_Player();
    }

    //Boolean operators for tree-------------------------------------------------

    Vector3 DetectCollision()
    {
        //check for object collisions with whiskers
        Vector3[] directions = { transform.forward,
                                    transform.forward + transform.up * collision_whisker_offset,
                                    transform.forward - transform.up * collision_whisker_offset,
                                    transform.forward + transform.right * collision_whisker_offset,
                                    transform.forward - transform.right * collision_whisker_offset,
                                    transform.forward + (transform.up + transform.right).normalized * collision_whisker_offset,
                                    transform.forward + (transform.up - transform.right).normalized * collision_whisker_offset,
                                    transform.forward - (transform.up + transform.right).normalized * collision_whisker_offset,
                                    transform.forward - (transform.up - transform.right).normalized * collision_whisker_offset,
                                };

        Vector3 avoidNormal = Vector3.zero;

        foreach (Vector3 dir in directions)
        {
            RaycastHit hitinfo;
            bool hit = Physics.Raycast(transform.position, transform.forward, out hitinfo, collision_whisker_len);

            if (hit)
            {
                float dist = Vector3.Distance(hitinfo.point, transform.position);

                avoidNormal += hitinfo.normal;
            }
        }

        if(avoidNormal.magnitude > 0)
        {
            return avoidNormal;
        }
        
        Vector3 fleeVector = Vector3.zero;

        //check for collisions with planes using personal space
        Vector3 toPlayer = player.transform.position - transform.position;
        if(toPlayer.magnitude <= collision_personal_space)
        {
            fleeVector -= toPlayer;
        }

        //check for collision with other enemies using  personal space
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            if(enemy != this)
            {
                Vector3 toEnemy = player.transform.position - transform.position;
                if (toEnemy.magnitude <= collision_personal_space)
                {
                    fleeVector -= toEnemy;
                }
            }
        }

        if(fleeVector.magnitude != 0)
        {
            return fleeVector;
        }
        return Vector3.zero;
    }

    //returns the missile closet missile that is targeting and traveling toward this fighter
    //null if there are none
    GameObject PlayerMissileApproaching()
    {
        GameObject result = null;
        float smallestAngle = Mathf.Infinity;

        GameObject[] missiles = GameObject.FindGameObjectsWithTag("Missile");

        foreach(GameObject missile in missiles)
        {
            Vector3 missileVel = missile.GetComponent<Rigidbody>().velocity;
            Vector3 missileToFighter = transform.position - missile.transform.position;
            float angle = Vector3.Angle(missileVel, missileToFighter);

            if(missile.GetComponent<PropNav>().Target == this && angle <= max_player_missile_approaching_angle && angle < smallestAngle)
            {
                result = missile;
                smallestAngle = angle;
            }
        }

        return result;
    }

    //true if missile is close to this fighter
    bool PlayerMissileClose(GameObject missile)
    {
        return Vector3.Distance(missile.transform.position, transform.position) <= max_player_missile_close_dist;
    }

    //true if a missile this fighter fired is close to the player
    bool MyMissileCloseToPlayer()
    {
        if (activeMissile == null)
        {
            return false;
        }

        return Vector3.Distance(activeMissile.transform.position, player.transform.position) <= max_my_missile_close_distance;
    }

    //true if a missile this fighter fired is close moving toward the player
    bool MyMissileApproachingPlayer()
    {
        if (activeMissile == null)
        {
            return false;
        }

        Vector3 missileVel = activeMissile.GetComponent<Rigidbody>().velocity;
        Vector3 missileToPlayer = player.transform.position - activeMissile.transform.position;
        float angle = Vector3.Angle(missileVel, missileToPlayer);

       if (activeMissile.GetComponent<PropNav>().Target == player && angle <= max_my_missile_approaching_angle)
       {
            return true;
       }

       return false;
    }

    //true if this fighter is within the player's targeting area and the fighter is visible to player
    bool InPlayerMissileCone()
    {
        Vector3 playerToFighter = transform.position - player.transform.position;
        float angle = Vector3.Angle(playerToFighter, player.transform.forward);
        return angle <= player_missile_cone_angle && playerToFighter.magnitude <= player_missile_cone_len && PlayerInLOS();
;
    }

    bool MissileAvailable()
    {
        return activeMissile == null;
    }

    //true if the player is within this fighter's targeting area and the player is visible to fighter
    bool PlayerInMissileCone()
    {
        Vector3 fighterToPlayer = player.transform.position - transform.position;
        float angle = Vector3.Angle(fighterToPlayer, transform.forward);
        return angle <= fighter_missile_cone_angle && fighterToPlayer.magnitude <= fighter_missile_cone_len && PlayerInLOS();
    }

    //true if a ray can be cast from this fighter to the player
    bool PlayerInLOS()
    {
        Vector3 towardPlayer = (player.transform.position - transform.position).normalized;

        RaycastHit hitinfo;
        bool hit = Physics.Raycast(transform.position, towardPlayer, out hitinfo);
        if (hit && hitinfo.collider.gameObject == player)
        {
            return true;
        }

        return false;
    }

    //Tree actions-----------------------------------------------------------

    //accel stright up or down depending on current orientation
    Vector3 Sharp_Pitch()
    {
        RaycastHit hitinfo;
        bool hit = Physics.Raycast(transform.position, -Vector3.up, out hitinfo);
        if(hit && hitinfo.distance <= min_pitch_ground_dist)
        {
            return Vector3.up;
        }

        //get vector from horizontal vector in world to the fighter's forward vector
        Vector3 diff = transform.forward - Vector3.forward;

        //if diff is pointing up, pitch up, otherwise, pitch down
        if(diff.y > 0)
        {
            return Vector3.up;
        }
        else
        {
            return -Vector3.up;
        }
    }

    //accelerate in the direction of missiles' right or left depending on current orientation
    Vector3 Turn_Side_Toward_Missile(GameObject missile)
    {
        Vector3 missile_to_fighter = transform.position - missile.transform.position;

        Vector3 diff = transform.forward - missile_to_fighter.normalized;
        
        if (diff.x > 0)
        {
            //right of the missile assuming it has zero rotation
            return new Vector3(missile.transform.right.x, 0, 0).normalized;
        }
        else
        {
            //left of the missile assuming it has zero rotation
            return new Vector3(-missile.transform.right.x, 0, 0).normalized;
        }
    }

    //maintain acceleration toward player
    Vector3 Seek_Player()
    {
        return (player.transform.position - transform.position).normalized;
    }


    Vector3 Exit_Missile_Cone()
    {
        Vector3 player_to_fighter = transform.position - player.transform.position;

        Vector3 diff = transform.forward - player_to_fighter.normalized;

        if (diff.x > 0)
        {
            //right of the player assuming it has zero rotation
            return new Vector3(player.transform.right.x, 0, 0).normalized;
        }
        else
        {
            //left of the player assuming it has zero rotation
            return new Vector3(-player.transform.right.x, 0, 0).normalized;
        }
    }

    //if player is far away from fighter, accelerate toward player
    //otherwise, if fighter is in front of player, accelerate in the oppoisite direction the player is facing
    //otherwise, pursue the player
    Vector3 Move_Behind_Player()
    {
        Vector3 player_to_fighter = player.transform.position - transform.position;
        if (player_to_fighter.magnitude >= min_far_away_dist)
        {
            return -player_to_fighter.normalized;
        }

        float angle = Vector3.Angle(player_to_fighter, player.transform.forward);

        if (angle <= max_in_front_of_player_angle)
        {
            return -player.transform.forward;
        }
        else
        {
            return -player_to_fighter.normalized;
        }
    }

    //if the player is on LOS, pursue it
    //otherwise, use Dijkstra's on the nodes to find the shortest path and seek the first node in that path
    Vector3 FollowPathToPlayer()
    {
        if(PlayerInLOS())
        {
            return player.transform.position - transform.position;
        }

        //get queue of all nodes and set up Dijkstra's
        List<GameObject> queue = new List<GameObject>(GameObject.FindGameObjectsWithTag("Node")); //replace with Dijkstrainfo
        foreach(GameObject node in queue)
        {
            node.GetComponent<DijkstraInfo>().Reset(this.gameObject, player);
        }
        GetComponent<DijkstraInfo>().Reset(this.gameObject, player);
        queue.Add(this.gameObject);
        player.GetComponent<DijkstraInfo>().Reset(this.gameObject, player);
        queue.Add(player);

        //set the source's info
        this.GetComponent<DijkstraInfo>().Dist = 0;
        
        //sort so shortest dist is in front
        queue.Sort(SortByDist);

        while (queue.Count != 0)
        {
            //get the node with least dist
            GameObject node = queue[0];
            queue.RemoveAt(0);

            //if that node is the player, we've found our path
            if(node == player)
            {
                break;
            }

            //for each neighbor still in queue, update the dist and prev if necessary
            List<GameObject> neighbors = node.GetComponent<DijkstraInfo>().Neighbors;
            foreach(GameObject neighbor in neighbors)
            {
                if(queue.Contains(neighbor))
                {
                    float alt = node.GetComponent<DijkstraInfo>().Dist + Vector3.Distance(node.transform.position, neighbor.transform.position);

                    if (alt < neighbor.GetComponent<DijkstraInfo>().Dist)
                    {
                        neighbor.GetComponent<DijkstraInfo>().Dist = alt;
                        neighbor.GetComponent<DijkstraInfo>().Prev = node;
                    }
                }
            }

            //sort again to puth the lowest dist node in front
            queue.Sort(SortByDist);
        }

        //follow the path backwards until you find the node in the path which is closest to this fighter
        GameObject closest_on_path = player;
        while(closest_on_path.GetComponent<DijkstraInfo>().Prev != this.gameObject)
        {
            //if a path was not found, just travel straight toward the player
            if(closest_on_path.GetComponent<DijkstraInfo>().Prev == null)
            {
                return player.transform.position - transform.position;
            }

            //otherwise, check the previous node
            closest_on_path = closest_on_path.GetComponent<DijkstraInfo>().Prev;
        }

        //return vector from the fighter to that node
        return closest_on_path.transform.position - transform.position;
    }

    static int SortByDist(GameObject n1, GameObject n2)
    {
        return n1.GetComponent<DijkstraInfo>().Dist.CompareTo(n2.GetComponent<DijkstraInfo>().Dist);
    }

    /*
    //predict the player's location, then accelerate towards it
    Vector3 Pursue_Player()
    {
        return Predict_Player_Loc() - transform.position;
    }

    //predict the player's location, then accelerate towards it
    Vector3 Evade_Player()
    {
        return transform.position - Predict_Player_Loc();
    }

    //predict the player's location at the time it would take for this fighter to reach it's current position
    Vector3 Predict_Player_Loc()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        float distToPlayer = toPlayer.magnitude;

        Vector3 fighterVel = GetComponent<Rigidbody>().velocity;

        float predictionTime;
        if (fighterVel.magnitude <= distToPlayer / max_prediction_time)
        {
            predictionTime = max_prediction_time;
        }
        else
        {
            predictionTime = distToPlayer / fighterVel.magnitude;
        }

        Vector3 playerVel = player.GetComponent<Rigidbody>().velocity;
        return player.transform.position + playerVel * predictionTime;
    }
    */
}
