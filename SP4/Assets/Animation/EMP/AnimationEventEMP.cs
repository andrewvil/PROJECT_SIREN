using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventEMP : MonoBehaviour
{
    [SerializeField] private EnemyEMP emp;
    private Collider col;

    private float timer = .5f;
    private bool canHit;
    private void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    public void Blast()
    {
        emp.Blast();
    }

    public void ColliderToggle()
    {
        if (col.enabled)
            col.enabled = false;
        else
            col.enabled = true;
    }

    public void AttackDone()
    {
        emp.AttackDone();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canHit)
        {
            canHit = false;
            if (GameManager.instance.bGameOver) return;
            GameManager.instance.lastHitEnemy = emp.jumpscareCamTransform.gameObject;
            emp.AttackPlayer();
        }
    }

    private void Update()
    {
        if (!canHit)
            timer -= Time.deltaTime;

        if (timer < 0)
            canHit = true;
    }
}
