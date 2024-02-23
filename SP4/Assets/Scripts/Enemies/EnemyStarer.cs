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

    [SerializeField] private AudioSource src;

    private bool goingUp = true;
    private float walkSpeed = 3.5f;
    private float chaseSpeed = 5.5f;
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
        allowAttack = true;
        speed = walkSpeed;
        isSeen = playerSpotted = false;
        agent = GetComponent<NavMeshAgent>();
        currentState = State.PATROL;
        target = patrolWaypoints[currWaypoint];
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.bGameOver) return;
        PlayerSeen();
        FSM();
    }

    //StateManager
    public override void FSM()
    {
        Debug.Log(patrolWaypoints[currWaypoint].name);
        if (isFlashed)
        {
            currWaypoint = Random.Range(0, patrolWaypoints.Count);
            transform.position = patrolWaypoints[currWaypoint].position;
            isFlashed = false;
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
                            Move(target);
                            currentState = State.PATROL;
                        }
                        //Chase
                        else if (playerSpotted)
                        {
                            target = playerTarget.transform;
                            speed = chaseSpeed;
                            if(!src.isPlaying)
                            {
                                src.Play();
                            }
                            Move(target);
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
                    if (Vector3.Distance(transform.position, target.transform.position) <= 0.75f)
                    {
                        speed = waitTime = 0;
                        currentState = State.IDLE;
                    }
                    //Chase
                    else if (playerSpotted)
                    {
                        target = playerTarget.transform;
                        speed = chaseSpeed;
                        if(!src.isPlaying)
                        {
                            src.Play();
                        }
                        Move(target);
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
                        GameManager.instance.deathTip = "Don't look away. Fight back with your camera.";
                        AttackPlayer();
                        speed = waitTime = 0;
                        agent.velocity= Vector3.zero;
                        src.Stop();
                        currentState = State.IDLE;
                    }
                }
                else
                {
                    speed = waitTime = 0;
                    src.Stop();
                    currentState = State.IDLE;
                }
                break;
            default:
                break;
        }
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
