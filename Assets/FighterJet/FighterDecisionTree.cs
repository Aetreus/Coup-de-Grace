﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FighterSteering))]
public class FighterDecisionTree : MonoBehaviour {

    public float accel_mag;

    //DetectCollision variables
    public float collision_whisker_len;
    public float collision_whisker_offset;
    public float collision_personal_space;

    //PlayerMissileApproaching variables
    public float max_player_missile_approaching_angle;
    public float beam_tolerance;

    //PlayerMissileClose variables
    public float max_player_missile_close_dist;
    public float stall_limit_excursion;

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

    //Maneuver parameter variables
    public float cruise_speed;
    public float maneuver_speed;
    public float sprint_speed;
    public float base_stall_limit;

    //Predict_Player_Loc variables
    //public float max_prediction_time;

    private GameObject targetNode;
    private float max_node_arrived_dist;

    private GameObject activeMissile = null;
    private GameObject target;

    private WeaponManager wm;

    private FighterSteering fs;
    private FlightBehavior fb;
    private Rigidbody rb;

    private AIAction _action;

    private string debug_msg;

    // Use this for initialization
    void Start() {
        target = GameObject.FindGameObjectWithTag("Player");
        wm = GetComponent<WeaponManager>();
        fs = GetComponent<FighterSteering>();
        fb = GetComponent<FlightBehavior>();
        rb = GetComponent<Rigidbody>();
        targetNode = null;
    }

    // Update is called once per frame
    void Update() {

        debug_msg = "";
        AIAction action = DetectCollisionTree();
        Debug.Log(debug_msg, this.gameObject);
        switch (action)
        {
            case AIAction.AVOID_COLLISION:
                break;
            case AIAction.AVOID_LOCK:
                Exit_Missile_Cone();
                break;
            case AIAction.AVOID_MISSILE:
                Break_Maneuver();
                break;
            case AIAction.MANEUVER_BEAM:
                Turn_Side_Toward_Missile(PlayerMissileApproaching());
                break;
            case AIAction.FIRE_MISSILE:
            case AIAction.MANEUVER_LOCK:
                Maintain_Lock();
                break;
            case AIAction.MANEUVER_REAR:
                Move_Behind_Player();
                break;
            case AIAction.PATH_PLAYER:
            default:
                break;

        }
    }

    public AIAction action
    {
        get
        {
            return _action;
        }
    }

    public enum AIAction
    {
        AVOID_COLLISION,
        AVOID_MISSILE,
        AVOID_LOCK,
        MANEUVER_REAR,
        MANEUVER_BEAM,
        MANEUVER_LOCK,
        FIRE_MISSILE,
        PATH_PLAYER
    }

    //forks of the tree------------------------------------------------------

    AIAction DetectCollisionTree()
    {
        Vector3 avoidVec = DetectCollision();

        if(avoidVec.magnitude > 0)
        {
            debug_msg += "Detect Collision T";
            return AIAction.AVOID_COLLISION;
        }
        else
        {
            debug_msg += "Detect Collision F";
            return PlayerMissileApproachingTree();
        }
    }

    AIAction PlayerMissileApproachingTree()
    {
        GameObject closest_missile = PlayerMissileApproaching();

        if(closest_missile != null)
        {
            debug_msg += " -> PlayerMissileApproaching T";
            Debug.DrawLine(transform.position, closest_missile.transform.position, Color.green);
            return PlayerMissileCloseTree(closest_missile);
        }
        else
        {
            debug_msg += " -> PlayerMissileApproaching F";
            return MyMissileApproachingPlayerTree();
        }
    }

    AIAction PlayerMissileCloseTree(GameObject closest_missile)
    {
        if(PlayerMissileClose(closest_missile))
        {
            debug_msg += " -> PlayerMissileClose T -> return Sharp Pitch";
            return AIAction.AVOID_MISSILE;
        }
        else
        {
            debug_msg += " -> PlayerMissileClose F";
            return MyMissileCloseToPlayerTree(closest_missile);
        }
    }

    AIAction MyMissileCloseToPlayerTree(GameObject closest_missile)
    {
        if(MyMissileCloseToPlayer())
        {
            debug_msg += " -> MyMissileCloseToPlayer T -> return Seek Player";
            return AIAction.MANEUVER_LOCK;
        }
        else
        {
            debug_msg += " -> MyMissileCloseToPlayer F -> return Turn_Side_Toward_Missile";
            return AIAction.MANEUVER_BEAM;
        }
    }

    AIAction MyMissileApproachingPlayerTree()
    {
        if(MyMissileApproachingPlayer())
        {
            debug_msg += " -> MyMissileApproachingPlayer T -> return Seek Player";
            return AIAction.MANEUVER_LOCK;
        }
        else
        {
            debug_msg += " -> MyMissileApproachingPlayer F";
            return PlayerInLOSTree();
        }
    }

    AIAction PlayerInLOSTree()
    {
        //return MissileAvailableTree();

        if (PlayerInLOS())
        {
            debug_msg += " -> PlayerInLOS T";
            return MissileAvailableTree();
        }
        else
        {
            debug_msg += " -> PlayerInLOS F -> return Follow_Path_To_Player";
            return AIAction.PATH_PLAYER;
        }
    }

    AIAction MissileAvailableTree()
    {
        if (MissileAvailable())
        {
            debug_msg += " -> MissileAvailable T";
            return PlayerInMissileConeTree();
        }
        else
        {
            debug_msg += " -> MissileAvailable F";
            return InPlayerMissileConeTree();
        }
    }

    AIAction PlayerInMissileConeTree()
    {
        if (PlayerInMissileCone())
        {
            debug_msg += " -> PlayerInMissileCone T -> Fire Missile";
            //fire missile
        }
        else
        {
            debug_msg += " -> PlayerInMissileCone F";
        }

        debug_msg += "-> return Seek_Player";
        return AIAction.MANEUVER_LOCK;
    }
    
    AIAction InPlayerMissileConeTree()
    {
        if(InPlayerMissileCone())
        {
            debug_msg += " -> InPlayerMissileCone T -> return Exit_Missile_Cone";
            return AIAction.AVOID_LOCK;
        }
        else
        {
            debug_msg += " -> InPlayerMissileCone F -> return Move_Behind_Player";
            return AIAction.MANEUVER_REAR;
        }
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
                                    transform.forward - (transform.up - transform.right).normalized * collision_whisker_offset
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
        Vector3 toPlayer = target.transform.position - transform.position;
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
                Vector3 toEnemy = target.transform.position - transform.position;
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

            if(missile.GetComponent<PropNav>().Target == this.gameObject && angle <= max_player_missile_approaching_angle && angle < smallestAngle)
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

        return Vector3.Distance(activeMissile.transform.position, target.transform.position) <= max_my_missile_close_distance;
    }

    //true if a missile this fighter fired is close moving toward the player
    bool MyMissileApproachingPlayer()
    {
        if (activeMissile == null)
        {
            return false;
        }

        Vector3 missileVel = activeMissile.GetComponent<Rigidbody>().velocity;
        Vector3 missileToPlayer = target.transform.position - activeMissile.transform.position;
        float angle = Vector3.Angle(missileVel, missileToPlayer);

        if (activeMissile.GetComponent<PropNav>().Target == target && angle <= max_my_missile_approaching_angle)
        {
            return true;
        }

       return false;
    }

    //true if this fighter is within the player's targeting area and the fighter is visible to player
    bool InPlayerMissileCone()
    {
        Vector3 playerToFighter = transform.position - target.transform.position;
        float angle = Vector3.Angle(playerToFighter, target.transform.forward);
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
        Vector3 fighterToPlayer = target.transform.position - transform.position;
        float angle = Vector3.Angle(fighterToPlayer, transform.forward);
        return angle <= fighter_missile_cone_angle && fighterToPlayer.magnitude <= fighter_missile_cone_len && PlayerInLOS();
    }

    //true if a ray can be cast from this fighter to the player
    bool PlayerInLOS()
    {
        Vector3 towardPlayer = target.transform.position - transform.position;

        RaycastHit hitinfo;
        bool hit = Physics.Raycast(transform.position, towardPlayer, out hitinfo, towardPlayer.magnitude);

        //Debug.DrawRay(transform.position, towardPlayer*200000, Color.green);

        if (hit && (hitinfo.collider.gameObject.tag == "Terrain" || hitinfo.collider.gameObject.tag == "Building"))
        {
            debug_msg += " [PlayerInLOS raycast hit " + hitinfo.collider.gameObject.name + "]";

            return false;
        }
        
        return true;
    }

    //Tree actions-----------------------------------------------------------

    //accel stright up or down depending on current orientation
    void Break_Maneuver()
    {
        RaycastHit hitinfo;
        bool hit = Physics.Raycast(transform.position, -Vector3.up, out hitinfo);
        if(hit && hitinfo.distance <= min_pitch_ground_dist)
        {
            fs.facingDir = Vector3.up;
            fs.upDir = Vector3.up;
            fs.stallLimit = stall_limit_excursion;
            fs.targetVel = sprint_speed;
        }

        //get vector from horizontal vector in world to the fighter's forward vector
        Vector3 diff = transform.forward - Vector3.forward;

        //if diff is pointing up, pitch up, otherwise, pitch down
        if (diff.y > 0)
        {
            fs.facingDir = Vector3.up;
            fs.upDir = Vector3.up;
            fs.stallLimit = stall_limit_excursion;
            fs.targetVel = sprint_speed;
        }
        else
        {
            fs.facingDir = -Vector3.up;
            fs.upDir = -Vector3.up;
            fs.stallLimit = stall_limit_excursion;
            fs.targetVel = sprint_speed;
        }
    }

    //accelerate in the direction of missiles' right or left depending on current orientation
    void Turn_Side_Toward_Missile(GameObject missile)
    {
        Vector3 fighter_to_missile = missile.transform.position - transform.position;

        //get the vector perpendicular to the vector toward the missile in the direction the fighter is currently facing
        Vector3 projection = Vector3.ProjectOnPlane(transform.forward, fighter_to_missile);

        //Get remaining angle for maneuver.
        float angle = Vector3.Angle(projection, transform.forward);

        //accelerate in that direciton horizontally
        fs.facingDir = projection;

        if (angle < beam_tolerance)
            fs.upDir = Vector3.up;
        else
        {
            float scaleFactor = Mathf.Pow(beam_tolerance / angle, 2);
            fs.facingDir = transform.forward + (Vector3.up - transform.forward) * scaleFactor;
        }

       

        /*
        Vector3 fighter_to_missile = missile.transform.position - transform.position;

        Vector3 projection = Vector3.ProjectOnPlane(fighter_to_missile, transform.forward);

        //Vector3 diff = transform.forward - fighter_to_missile.normalized;

        float dir = AngleDir(fighter_to_missile.normalized, transform.forward, Vector3.up);
        
        if (dir > 0) //fighter is facing to the right of the missile
        {
            Vector3 rightOfMissile = missile.transform.forward + Vector3.right;
            //right of the missile assuming it has zero rotation
            return new Vector3(missile.transform.right.x, 0, 0).normalized;
        }
        else //fighter is facing to the left of the missile
        {
            //left of the missile assuming it has zero rotation
            return new Vector3(-missile.transform.right.x, 0, 0).normalized;
        }
        */
    }

    //maintain acceleration toward player
    void Maintain_Lock()
    {
        //Aim ahead of the target depending on its velocity, but keep it in the lock cone.
        Vector3 leadFacing = target.transform.position + target.GetComponent<Rigidbody>().velocity * (float)((target.transform.position - transform.position).magnitude / rb.velocity.magnitude) - transform.position;
        leadFacing = leadFacing.normalized;
        Vector3 directFacing = (target.transform.position - transform.position).normalized;

        float angle = Vector3.Angle(leadFacing, directFacing);

        float scaleTerm = 1;

        scaleTerm = Mathf.Min(scaleTerm, Mathf.Sin(Mathf.Deg2Rad* 25) / Mathf.Sin(Mathf.Deg2Rad * angle));

        Vector3 output = directFacing + (leadFacing - directFacing) * scaleTerm;

        output = output.normalized;

        fs.facingDir = output;
        fs.upDir = output;
    }


    void Exit_Missile_Cone()
    {
        Vector3 fighter_to_player = target.transform.position - transform.position;

        //get the vector perpendicular to the vector toward the player in the fighter is currently facing
        Vector3 projection = Vector3.ProjectOnPlane(transform.forward, fighter_to_player);

        //accelerate in the opposite direction of that direction projection
        fs.facingDir = projection.normalized;
        fs.upDir = projection.normalized;


        /*
        Vector3 diff = fighter_to_player.normalized - transform.forward;

        return -diff;

        //Vector3 diff = player_to_fighter.normalized - player.transform.forward;
        
        float dir = AngleDir(fighter_to_player.normalized, transform.forward, Vector3.up);

        if (dir > 0) //fighter is facing to the right of the player
        {
            //right of the player assuming it has zero rotation
            return new Vector3(player.transform.right.x, 0, 0).normalized;
        }
        else //fighter is facing to the left of the player
        {
            //left of the player assuming it has zero rotation
            return new Vector3(-player.transform.right.x, 0, 0).normalized;
        }
        */
    }

    //if player is far away from fighter, accelerate toward player
    //otherwise, if fighter is in front of player, accelerate in the oppoisite direction the player is facing
    //otherwise, pursue the player
    void Move_Behind_Player()
    {
        Vector3 player_to_fighter = transform.position - target.transform.position;

        //if the fighter is far away, just move toward it
        if (player_to_fighter.magnitude >= min_far_away_dist)
        {
            debug_msg += " [far away, move toward player]";
            fs.facingDir = player_to_fighter;
            fs.targetVel = 300;
        }

        float angle = Vector3.Angle(player_to_fighter, target.transform.forward);

        //if the fighter is in front of the player,
        //move in the opposite direction the player is facing
        if (angle <= max_in_front_of_player_angle)
        {
            debug_msg += " [in front of player, move behind it]";
            fs.facingDir = -target.transform.forward;
            fs.upDir = (transform.forward - target.transform.forward).normalized;
            fs.targetVel = 200;
        }
        //otherwise, move toward the fighter
        else
        {
            debug_msg += " [behind player and close, move chase it]";
            fs.facingDir = -player_to_fighter.normalized;
            //If the player is close in heading to us, match up vectors to anticipate a turn. Otherwise align to turn onto him.
            if (Vector3.Angle(target.transform.forward, transform.forward) < 5)
                fs.upDir = target.transform.up;
            else
                fs.upDir = (transform.forward - target.transform.forward).normalized;
            //Try to maintain the distance between 1000-2000m
            if (player_to_fighter.magnitude > 2000)
            {
                fs.targetVel = target.GetComponent<Rigidbody>().velocity.magnitude + 100;
            }
            else if (player_to_fighter.magnitude < 1000)
            {
                fs.targetVel = target.GetComponent<Rigidbody>().velocity.magnitude - 50;
            }
            else
            {
                fs.targetVel = target.GetComponent<Rigidbody>().velocity.magnitude;
            }
        }
    }

    //if the player is on LOS, pursue it
    //otherwise, use Dijkstra's on the nodes to find the shortest path and seek the first node in that path
    void Follow_Path_To_Player()
    {
        if(PlayerInLOS())
        {
            debug_msg += "-> PlayerInLOS";
            fs.facingDir = (target.transform.position - transform.position).normalized;
            fs.upDir = (target.transform.position - transform.position).normalized;

        }

        //if not arrived at previously calculated next node yet, then keep traveling towards it
        if(targetNode != null && Vector3.Distance(transform.position, targetNode.transform.position) > max_node_arrived_dist)
        {
            debug_msg += "-> not at previously found node";
            fs.facingDir = targetNode.transform.position - transform.position;
            fs.upDir = targetNode.transform.position - transform.position;
        }

        //get queue of all nodes and set up Dijkstra's
        List<GameObject> queue = new List<GameObject>(GameObject.FindGameObjectsWithTag("Node")); //replace with Dijkstrainfo
        foreach(GameObject node in queue)
        {
            node.GetComponent<DijkstraInfo>().Reset(this.gameObject, target);
        }
        queue.Remove(targetNode);
        GetComponent<DijkstraInfo>().Reset(this.gameObject, target);
        queue.Add(this.gameObject);
        target.GetComponent<DijkstraInfo>().Reset(this.gameObject, target);
        queue.Add(target);


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
            if(node == target)
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
        GameObject closest_on_path = target;
        while(closest_on_path.GetComponent<DijkstraInfo>().Prev != this.gameObject)
        {
            //if a path was not found, just travel straight toward the player
            if(closest_on_path.GetComponent<DijkstraInfo>().Prev == null)
            {
                fs.facingDir = target.transform.position - transform.position;
                fs.upDir = target.transform.position - transform.position;
            }

            //otherwise, check the previous node
            closest_on_path = closest_on_path.GetComponent<DijkstraInfo>().Prev;
        }

        //return vector from the fighter to that node and set it as the targetNode
        debug_msg += "-> found new node";
        targetNode = closest_on_path;
        fs.facingDir = closest_on_path.transform.position - transform.position;
        fs.upDir = closest_on_path.transform.position - transform.position;
    }

    //helper functions--------------------------------------------------

    static int SortByDist(GameObject n1, GameObject n2)
    {
        return n1.GetComponent<DijkstraInfo>().Dist.CompareTo(n2.GetComponent<DijkstraInfo>().Dist);
    }

    //returns -1 when to the left, 1 to the right, and 0 for forward/backward
    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f)
        {
            return 1.0f;
        }
        else if (dir < 0.0f)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
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
