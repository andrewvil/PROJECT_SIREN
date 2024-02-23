using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BlindEnemy : EnemyBase, IAudioObserver, ISightObserver, IPhotoObserver
{
    [SerializeField]
    Vector3 audioPosition = Vector3.zero;

    //[SerializeField]
    //private List<Transform> waypoints;

    [SerializeField]
    private GameObject jumpscareCam;

    [SerializeField]
    int waypointIndex;

    [SerializeField]
    private bool isChasingAudio;

    [SerializeField]
    private Animator animator;

    //year
    private GameObject targetObject;

    Coroutine patrolCooldownCoroutine;

    [SerializeField] private AudioSource src;

    [SerializeField] private AudioClip chaseClip;
    [SerializeField] private AudioClip idleClip;

    public override void FSM()
    {
        if(isChasingAudio)
        {
            if(patrolCooldownCoroutine==null)
                patrolCooldownCoroutine = StartCoroutine(ReturnToPatrol());
            if(Vector3.Distance(audioPosition,transform.position) > 1)
            {
                MoveToPos(audioPosition);
                animator.SetInteger("Move", 2);
            }
            
        }
        else
        {
            if(Vector3.Distance(patrolWaypoints[waypointIndex].position,transform.position) < 1)
            {
                waypointIndex = UnityEngine.Random.Range(0,patrolWaypoints.Count);
            }
            else
            {
                MoveToPos(patrolWaypoints[waypointIndex].position);
                animator.SetInteger("Move", 1);
            }
        }
    }

    private IEnumerator ReturnToPatrol()
    {
        Debug.Log("returning to patrol...");
        yield return new WaitForSeconds(3f);
        speed = 3f;
        isChasingAudio = false;
        targetObject = null;
        patrolCooldownCoroutine = null;
        src.PlayOneShot(idleClip);
        src.Play();
    }

    public void Notify(Vector3 position, GameObject source)
    {
        if(targetObject != null)
        {
            if(targetObject.CompareTag("Decoy"))
            {
                return;
            }
        }
        targetObject = source;
        if(targetObject.CompareTag("Player"))
        {
            target = targetObject.transform;
        }
        Debug.Log("hi");
        audioPosition = position;
        isChasingAudio = true;
        speed = 10f;

        src.PlayOneShot(chaseClip);
        if(!src.isPlaying) src.Play();
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = 3f;
        isChasingAudio = false;
        waypointIndex = 0;
        damage = 50;
    }
    void Update()
    {
        if(GameManager.instance.bGameOver) {    
            if(src.enabled)
                src.enabled = false;
            return;
        }
        FSM();
    }

    public void OnSighted()
    {
    }

    public void OnLookAway()
    {
    }

    public void OnPhotoTaken()
    {
        UIManager.instance.DisplayTip("EYELESS", "BLIND BUT EXTREMELY SENSITIVE TO SOUND.\nPROCEED WITH CAUTION.", true, true);
    }

    public string GetDetails()
    {
        return "DANGER";
    }

    void Start()
    {
        damage = 35;
        allowAttack = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("i hit player");
            GameManager.instance.lastHitEnemy = jumpscareCam;
            GameManager.instance.deathTip = "Don't make a sound around the EYELESS.";
            AttackPlayer();
            src.Stop();
        }
    }
}
