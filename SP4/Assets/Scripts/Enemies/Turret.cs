using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : EnemyBase
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject warningLight;
    [SerializeField] private float idleTime;
    [SerializeField] private Vector3 rotationDest1;
    [SerializeField] private Vector3 rotationDest2;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float windUpTime;
    [SerializeField] private float rateOfFire;

    [SerializeField] private AudioClip windUpSFX;
    [SerializeField] private AudioClip fireSFX;

    private enum State
    {
        IDLE,
        SCANNING,
        TARGETSPOTTED,
        SHOOT,
    }

    private AudioSource src;
    private State currState;
    private bool targetSpotted = false;
    private float idleTimer;
    private Vector3 targetRotation;
    private float windUpTimer;
    private float timeBetweenShots;
    private float shootTimer;

    private void Start()
    {
        src = GetComponent<AudioSource>();

        timeBetweenShots = 1 / rateOfFire;
        targetRotation = rotationDest2;

        ChangeState(State.IDLE);
    }

    private void Update()
    {
        if(GameManager.instance.bGameOver) return;
        FSM();
    }

    private void ChangeState(State newState)
    {
        if (newState == State.IDLE)
        {
            idleTimer = idleTime;
        }
        else if (newState == State.SCANNING)
        {
            if (targetRotation == rotationDest2)
                targetRotation = rotationDest1;
            else
                targetRotation = rotationDest2;
        }
        else if (newState == State.TARGETSPOTTED)
        {
            warningLight.SetActive(true);
            windUpTimer = windUpTime;
            src.PlayOneShot(windUpSFX);
        }
        else if (newState == State.SHOOT)
        {
            shootTimer = timeBetweenShots;
        }

        currState = newState;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            
            if (Physics.Linecast(firePoint.position, target.position, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject.CompareTag("Player"))
                    targetSpotted = true;
                else
                    targetSpotted = false;
            }
        }
    }

    public override void FSM()
    {
        if (currState == State.IDLE)
        {
            idleTimer -= Time.deltaTime;
            if (targetSpotted)
                ChangeState(State.TARGETSPOTTED);
            else if (idleTimer <= 0)
                ChangeState(State.SCANNING);
        }
        else if (currState == State.SCANNING)
        {
            Vector3 rot = Vector3.Slerp(transform.localRotation.eulerAngles, targetRotation, rotationSpeed * 0.1f);
            transform.localRotation = Quaternion.Euler(rot);

            if (targetSpotted)
                ChangeState(State.TARGETSPOTTED);
            else if (transform.localRotation == Quaternion.Euler(targetRotation))
                ChangeState(State.IDLE);
        }
        else if (currState == State.TARGETSPOTTED)
        {
            transform.LookAt(target);

            windUpTimer -= Time.deltaTime;

            if (windUpTimer <= 0)
                ChangeState(State.SHOOT);
            else if (!targetSpotted)
            {
                warningLight.SetActive(false);
                ChangeState(State.IDLE);
            }
        }
        else if (currState == State.SHOOT)
        {
            transform.LookAt(target);

            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
            {
                AttackPlayer();
                src.PlayOneShot(fireSFX);
                shootTimer = timeBetweenShots;
            }
            else if (!targetSpotted)
            {
                warningLight.SetActive(false);
                ChangeState(State.IDLE);
            }
        }
    }

    public override void AttackPlayer()
    {
        GameManager.instance.deathTip = "Avoid the turret's line of fire";
        target.GetComponent<IHealth>().UpdateHealth(-damage);
    }
}
