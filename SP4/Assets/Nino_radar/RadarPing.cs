using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarPing : MonoBehaviour {

    private Material material;
    private float disappearTimer;
    private float disappearTimerMax;
    private Color color;
    private float emmisionIntensity;

    private void Start() 
    {
        material = GetComponent<MeshRenderer>().material;
        material.EnableKeyword("_EMISSION");
        disappearTimerMax = 1.5f;
        disappearTimer = 0f;
        color = new Color(1, 0, 0, 1);
    }

    private void Update() 
    {
        disappearTimer += Time.deltaTime;
        
        emmisionIntensity = Mathf.Lerp(disappearTimerMax, 0f, disappearTimer / disappearTimerMax);
        material.SetColor("_BaseColor", color * emmisionIntensity);
        material.SetColor("_EmissionColor", color * emmisionIntensity);

        if (disappearTimer >= disappearTimerMax) {
            Destroy(gameObject);
        }
    }
}
