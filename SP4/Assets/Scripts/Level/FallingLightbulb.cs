using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingLightbulb : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private int damage;

    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip hitClip;

    private bool triggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!triggered)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius, hitLayer);
            foreach(Collider hit in hits)
            {
                if (hit.GetComponent<IHealth>() != null)
                {
                    GameManager.instance.lastHitEnemy = null;
                    hit.GetComponent<IHealth>().UpdateHealth(-damage);
                }
            }
            triggered = true;
            src.PlayOneShot(hitClip);
        }
    }
}
