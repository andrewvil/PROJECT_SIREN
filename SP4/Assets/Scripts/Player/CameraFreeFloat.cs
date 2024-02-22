using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFreeFloat : MonoBehaviour
{
    float mouseX, mouseY;
    float moveX, moveY;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mouseX = mouseY = 0f;
    }


    // Update is called once per frame
    void Update()
    {
        mouseX += Input.GetAxis("Mouse X");
        mouseY += -Input.GetAxis("Mouse Y");
        mouseY = Mathf.Clamp(mouseY, -89f, 89f);
        Camera.main.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0f);

        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
        Vector3 moveDir = Camera.main.transform.forward * moveY + Camera.main.transform.right * moveX;
        Camera.main.transform.position += moveDir * Time.deltaTime * 7.5f;
    }
}
