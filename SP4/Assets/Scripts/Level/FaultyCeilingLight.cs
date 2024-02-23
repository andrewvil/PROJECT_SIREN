using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaultyCeilingLight : MonoBehaviour
{
    [SerializeField] private GameObject objToDrop;
    [SerializeField] private float timeToFall;
    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject sparks;
    [SerializeField] private float timeBetweenFlash;

    private float timeElapsed = 0f;
    private bool triggered = false;
    private float flashTimeElapsed = 0f;
    private bool lightActive = true;

    private void Start()
    {
        indicator.SetActive(lightActive);
        sparks.SetActive(lightActive);
    }
    
    private void Update()
    {
        if (triggered && timeElapsed < timeToFall)
            timeElapsed += Time.deltaTime;
        if (timeElapsed >= timeToFall)
            objToDrop.SetActive(true);
        if (!triggered)
            flashTimeElapsed += Time.deltaTime;
        if (flashTimeElapsed >= timeBetweenFlash)
        {
            flashTimeElapsed = 0f;
            lightActive = !lightActive;
            indicator.SetActive(lightActive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Play short circuit SFX
            triggered = true;
            lightActive = false;
            indicator.SetActive(lightActive);
            sparks.SetActive(lightActive);
        }
    }
}
