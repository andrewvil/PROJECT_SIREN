using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyEMP : EnemyBase, ISightObserver, IPhotoObserver
{
    public Transform jumpscareCamTransform;

    [SerializeField] private Transform player;
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float chaseSpeed = 10f;

    [SerializeField] private GameObject blastPrefab;

    [SerializeField]
    private float patrolTimerSet = 30f;
    private float chaseTimerSet = 30f;
    public float restTimerSet = 50f;
    public float timer;

    public bool active = true;

    private int chargeCount = 0;

    [SerializeField] private Light lightsource;
    [SerializeField] private SkinnedMeshRenderer mr;

    private Color passiveLight;
    private Color angryLight;
    private float passiveIntensity = 1;
    private float angryIntensity = 100;
    private Color passiveLightEmission;
    private Color angryLightEmission;

    [SerializeField]
    private AudioSource src;

    [SerializeField]
    private AudioClip empExplosionClip;

    private enum State
    {
        PATROL,
        CHARGE,
        CHASE,
        ATTACK,
        REST
    }

    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        allowAttack = true;
        timer = patrolTimerSet;

        speed = walkSpeed;

        ar = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentState = State.PATROL;
        currWaypoint = 0;
        target = patrolWaypoints[currWaypoint];

        passiveLight = new Color(1, 0.9375429f, 0.6132075f);

        angryLight = new Color(1, 0, 0);

        passiveLightEmission = new Color(1.232728f, 0.7357641f, 0.180714f);
        
        angryLightEmission = new Color(2.813975f, 0.05163257f, 0f);

        lightsource.color = passiveLight;
        lightsource.intensity = passiveIntensity;
        mr.material.SetColor("_EmissionColor", passiveLightEmission);
        mr.material.EnableKeyword("_EMISSION");

    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            Debug.Log("deactivated");
            speed = 0;
            chargeCount = 0;
            ar.SetTrigger("rest");
            ar.SetBool("charge", false);
            ar.SetBool("chase", false);
            currentState = State.REST;
        }

        FSM();

        Move(target);

        ar.SetFloat("move", agent.velocity.normalized.magnitude);
    }

    public override void FSM()
    {
        switch (currentState)
        {
            case State.PATROL:
                timer -= Time.deltaTime;
                speed = walkSpeed;
                if (timer <= 0f && Random.Range(1, 100) > 40)
                {
                    currentState = State.CHARGE;
                    break;
                }
                else if (timer > 0)
                {
                    target = patrolWaypoints[currWaypoint];
                }
                else
                    timer = patrolTimerSet;

                if (Vector3.Distance(transform.position, target.transform.position) < .75f)
                    currWaypoint = Random.Range(0, patrolWaypoints.Count);
                break;
            case State.CHARGE:
                lightsource.color = Color.Lerp(lightsource.color, angryLight, Time.deltaTime / 15);
                lightsource.intensity = Mathf.Lerp(lightsource.intensity, angryIntensity, Time.deltaTime / 15);

                mr.material.SetColor("_EmissionColor", Color.Lerp(mr.material.GetColor("_EmissionColor"), angryLightEmission, Time.deltaTime / 15));
                mr.material.EnableKeyword("_EMISSION");
                ar.SetBool("charge", true);
                speed = 0;
                if (chargeCount == 4 && Vector3.Distance(transform.position, player.transform.position) < 30)
                {
                    timer = chaseTimerSet;
                    ar.SetBool("charge", false);
                    currentState = State.CHASE;
                    chargeCount = 0;
                    break;
                }
                break;
            case State.CHASE:
                ar.SetBool("chase", true);
                speed = chaseSpeed;
                timer -= Time.deltaTime;

                if (Vector3.Distance(transform.position, target.transform.position) < 3)
                {
                    ar.SetBool("chase", false);
                    currentState = State.ATTACK;
                    break;
                }

                if (timer <= 0)
                {
                    timer = restTimerSet;
                    ar.SetBool("chase", false);
                    currentState = State.REST;
                    break;
                }

                speed = chaseSpeed;
                target = player;

                break;
            case State.ATTACK:
                speed = 0;
                agent.velocity = Vector3.zero;
                gameObject.transform.LookAt(target);
                ar.SetTrigger("attack");

                break;
            case State.REST:
                lightsource.color = Color.Lerp(lightsource.color, passiveLight, Time.deltaTime);
                lightsource.intensity = Mathf.Lerp(lightsource.intensity, passiveIntensity, Time.deltaTime);

                mr.material.SetColor("_EmissionColor", Color.Lerp(mr.material.GetColor("_EmissionColor"), passiveLightEmission, Time.deltaTime));
                mr.material.EnableKeyword("_EMISSION");
                active = false;
                speed = 0;
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    active = true;
                    timer = patrolTimerSet;
                    currentState = State.PATROL;
                }

                break;
            default:
                break;
        }
    }

    public void Blast()
    {
        Instantiate(blastPrefab, transform);
        src.PlayOneShot(empExplosionClip);
        chargeCount++;
    }

    public void AttackDone()
    {
        if (timer > 0)
        {
            currentState = State.CHASE;
        }
        else
        {
            ar.SetTrigger("rest");
            currentState = State.REST;
        }
    }

    public void OnSighted()
    {
    }

    public void OnLookAway()
    {
    }

    public void OnPhotoTaken()
    {
        UIManager.instance.DisplayTip("EMP", "This anomaly absorbs electricity around it to power itself. It has a deactivate switch on its back but no one has ever been able to touch it.", true, true);
    }

    public string GetDetails()
    {
        return "DANGER";
    }
}
