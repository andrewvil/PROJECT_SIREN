using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float mouseX, mouseY;

    [SerializeField]
    private float mouseSensitivity = 1f;

    [SerializeField] private float timeBetweenShakes;
    [SerializeField] private float shakeMagnitude;
    
    Vector3 originalPos;

    Coroutine shakeCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mouseX = mouseY = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.isInUI) return;
        if(GameManager.instance.bGameOver) return;
        mouseX += Input.GetAxis("Mouse X")* mouseSensitivity;
        mouseY += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -89f, 89f);
        Camera.main.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0f);
    }

    private IEnumerator CameraShake()
    {
        float x = originalPos.x + Random.Range(-1f, 1f) * shakeMagnitude;
        float y = originalPos.y + Random.Range(-1f, 1f) * shakeMagnitude;
        float z = originalPos.z + Random.Range(-1f, 1f) * shakeMagnitude;

        Camera.main.transform.localPosition = new Vector3(x, y, z);

        yield return new WaitForSeconds(timeBetweenShakes);

        shakeCoroutine = StartCoroutine(CameraShake());
    }

    public void StartShake()
    {
        originalPos = new Vector3(0f,0.5f,0f);
        shakeCoroutine = StartCoroutine(CameraShake());
    }

    public void StopShake()
    {
        if(shakeCoroutine == null) return;
        StopCoroutine(shakeCoroutine);
        shakeCoroutine = null;
        Camera.main.transform.localPosition = new Vector3(0f,0.5f,0f);
    }

    public void SetShakeMagnitude(float newShakeMagnitude)
    {
        shakeMagnitude = newShakeMagnitude;
    }

    public float GetShakeMagnitude()
    {
        return shakeMagnitude;
    }
}
