using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyelessAttackCollisionHandler : MonoBehaviour
{
    [SerializeField]
    private BoxCollider attackCollider;
    [SerializeField]
    private BlindEnemy eyeless;

    // Start is called before the first frame update
    void Start()
    {
        attackCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCollider.enabled)
        {
            Collider[] radarColliders = Physics.OverlapBox(attackCollider.center, attackCollider.size, Quaternion.identity);
            foreach (var collider in radarColliders)
            {
                if (collider.gameObject.CompareTag("Player"))
                {
                    attackCollider.enabled = false;
                    eyeless.AttackPlayer();
                    break;
                }
            }
        }
    }

    public void EnableCollider()
    {
        attackCollider.enabled = true;
    }
}
