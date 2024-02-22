using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastEMP : MonoBehaviour
{
    void Update()
    {
        if (transform.localScale.x < 30)
            transform.localScale = new Vector3(transform.localScale.x + Time.deltaTime * 50, transform.localScale.y + Time.deltaTime * 50, transform.localScale.z + Time.deltaTime * 50);
        else
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.GetComponent<PlayerController>()?.DisableEquipment();
    }
}
