using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MMNVManager : MonoBehaviour
{
    [SerializeField] private Volume volume;
    private float timer;
    private float timerSet = 2f;
    bool inNV = false, maxNV = false;

    private void Start()
    {
        timer = timerSet;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        Debug.Log(timer);
        if (inNV)
        {
            volume.profile.TryGet(out NightVisionPostProcess nv);
            if (nv.nightVisionIntensity.value < 10.9 && !maxNV)
            {
                nv.nightVisionIntensity.value = Mathf.Lerp(nv.nightVisionIntensity.value, 11, Time.deltaTime);
            }
            else
            {
                maxNV = true;
                nv.nightVisionIntensity.value = Mathf.Lerp(nv.nightVisionIntensity.value, 0, Time.deltaTime * 5);
                if (nv.nightVisionIntensity.value < 0.1)
                {
                    nv.nightVisionIntensity.value = 0;
                    inNV = false;
                    timer = timerSet;
                }
            }

        }

        if (timer < 0 && !inNV)
        {
            if (Random.Range(0, 100) > 50)
            {
                maxNV = false;
                inNV = true;
            }
            else
                timer = timerSet;
        }
    }
}
