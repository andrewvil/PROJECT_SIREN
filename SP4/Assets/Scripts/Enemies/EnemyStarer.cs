using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStarer : EnemyBase, ISightObserver, IPhotoObserver
{
    [SerializeField]
    private GameObject playerTarget;
    [SerializeField]
    private Transform eyeSight;
    [SerializeField]
    private Transform jumpscareCamTransform;

    private bool goingUp = true;
    private float walkSpeed = 3.5f;
    private float chaseSpeed = 22.5f;
    private float waitTime = 0.0f;

    public bool playerSpotted;
    public bool isSeen;
    public bool isFlashed;

    private enum State
    {
        IDLE,
        PATROL,
        CHASE
    }
    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        damage = 20;
        speed = walkSpeed;
        isSeen = playerSpotted = false;
        agent = GetComponent<NavMeshAgent>();
        currentState = State.PATROL;
        target = patrolWaypoints[currWaypoint];
    }

    // Update is called once per frame
    void Update()
    {
        PlayerSeen();
        FSM();
    }

    //StateManager
    public override void FSM()
    {
        if (isFlashed)
        {
            currWaypoint = Random.Range(0, patrolWaypoints.Count);
            Move(patrolWaypoints[currWaypoint]);
            return;
        }
        switch (currentState)
        {
            case State.IDLE:
                waitTime += Time.deltaTime;
                if (waitTime >= 1.0f)
                {
                    if (!isSeen)
                    {
                        //Patrol
                        if (!playerSpotted)
                        {
                            currWaypoint = goingUp ? (currWaypoint + 1) : (currWaypoint - 1);
                            if ((currWaypoint + 1) > (patrolWaypoints.Count - 1) && goingUp || (currWaypoint - 1) < 0 && !goingUp)
                            {
                                goingUp = !goingUp;
                            }
                            target = patrolWaypoints[currWaypoint];
                            speed = walkSpeed;
                            currentState = State.PATROL;
                        }
                        //Chase
                        else if (playerSpotted)
                        {
                            target = playerTarget.transform;
                            speed = chaseSpeed;
                            currentState = State.CHASE;
                        }
                    }
                    //Spotted by player
                    else
                    {
                        waitTime = 0.0f;
                    }
                }
                break;
            case State.PATROL:
                if (!isSeen)
                {
                    //Idle
                    if (agent.remainingDistance <= 0.75f)
                    {
                        speed = waitTime = 0;
                        currentState = State.IDLE;
                    }
                    //Chase
                    else if (playerSpotted)
                    {
                        target = playerTarget.transform;
                        speed = chaseSpeed;
                        currentState = State.CHASE;
                    }
                }
                //Spotted by player
                else
                {
                    speed = waitTime = 0;
                    currentState = State.IDLE;
                }
                break;
            case State.CHASE:
                if (!isSeen) 
                {
                    //Attack
                    if (agent.remainingDistance <= 0.8f)
                    {
                        GameManager.instance.lastHitEnemy = jumpscareCamTransform.gameObject;
                        AttackPlayer();
                        speed = waitTime = 0;
                        agent.velocity= Vector3.zero;
                        currentState = State.IDLE;
                    }
                }
                else
                {
                    speed = waitTime = 0;
                    currentState = State.IDLE;
                }
                break;
            default:
                break;
        }
        Move(target);
    }

    //See Player
    public void PlayerSeen()
    {
        if (Physics.Linecast(eyeSight.position, playerTarget.transform.position, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.DrawLine(eyeSight.position, hit.point, Color.green);
                playerSpotted = true;
            }
            else
            {
                Debug.DrawLine(eyeSight.position, hit.point, Color.red);
                playerSpotted = false;
            }
        }
    }

    //CameraFlash
    public void OnPhotoTaken()
    {
        
        UIManager.instance.DisplayTip("Starer", "Don't. Look. Away.", true, true);
        isFlashed = true;
    }

    //Seen
    public void OnLookAway()
    {
        isSeen = false;
    }
    public void OnSighted()
    {
        Invoke(nameof(Freeze), .05f);
    }
    public void Freeze()
    {
        isSeen = true;
    }

    public string GetDetails() => "DANGER";
}
