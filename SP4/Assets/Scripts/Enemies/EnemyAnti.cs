using System;
using System.Collections;

using UnityEngine;
using UnityEngine.AI;

public class EnemyAnti : EnemyBase
{
    [Header("Anti-NV")]
    [SerializeField] private GameObject modelObject;
    [SerializeField] private float rageBuildUp = 1f;
    [SerializeField] private GameObject deathCam;
    
    [Header("Audio")]
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip idleSound;

    private enum State
    {
        IDLE,
        RAGE,
    }

    private Material material;
    private State currState;
    private float rage;

    private void Start()
    {
        speed = 10f;
        material = modelObject.GetComponent<Renderer>().material;
        agent = GetComponent<NavMeshAgent>();

        ChangeState(State.IDLE);
        src.PlayOneShot(idleSound);
        allowAttack = true;
    }

    private void Update()
    {
        if(GameManager.instance.bGameOver) return;
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        transform.forward = direction;
        
        FSM();
    }

    Coroutine chaseCoroutine;
    private void ChangeState(State newState)
    {
        if (newState == State.IDLE)
        {
            rage = 0f;
            material.SetFloat("_REFLECTIONS_WEIGHT", rage);
        }
        else if (newState == State.RAGE)
        {
            chaseCoroutine = StartCoroutine(ChaseDuration());
        }

        currState = newState;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(GameManager.instance.bGameOver) 
            return;
            
        if (currState == State.RAGE)
        {
            GameManager.instance.lastHitEnemy = deathCam;
            GameManager.instance.deathTip = "Be careful when you activate night vision";
            AttackPlayer();
            StopCoroutine(chaseCoroutine);
        }
    }

    public override void FSM()
    {
        if (currState == State.IDLE)
        {
            rage += rageBuildUp * Time.deltaTime;
            material.SetFloat("_REFLECTIONS_WEIGHT", rage);

            if (rage >= 50f)
                ChangeState(State.RAGE);
        }
        else if (currState == State.RAGE)
        {
            Move(target);
        }
    }

    private IEnumerator ChaseDuration()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
