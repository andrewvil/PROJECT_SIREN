using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLeverDoor : MonoBehaviour
{
    [SerializeField] private GameObject door1;
    [SerializeField] private GameObject door2;
    [SerializeField] private int powerThreshold;
    [SerializeField] private Collider doorCollider;

    bool isOpen = false;
    private void Open()
    {
        isOpen = true;
        doorCollider.enabled = !isOpen;
        //TODO: Open
    }

    private void Close()
    {
        isOpen = false;
        doorCollider.enabled = !isOpen;
        //TODO: Close
    }
    private int power;

    void Awake()
    {
        power = 0;
        doorCollider.enabled = true;
    }

    public void AddPower()
    {
        power++;
        if(power>=powerThreshold)
        {
            Open();
        }
    }

    public void RemovePower()
    {
        power--;
        if(power<powerThreshold)
        {
            Close();
        }
    }

    [SerializeField]
    private float openSpeed;
    void Update()
    {
        door1.transform.localPosition = Vector3.Lerp(door1.transform.localPosition,isOpen?new Vector3(-2f,0f,0f):new Vector3(-1f,0f,0f), openSpeed);
        door2.transform.localPosition = Vector3.Lerp(door2.transform.localPosition,isOpen?new Vector3(2f,0f,0f):new Vector3(1f,0f,0f), openSpeed);
    }
}
