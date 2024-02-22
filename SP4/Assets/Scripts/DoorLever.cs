using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLever : MonoBehaviour, IInteract, ISightObserver
{
    [SerializeField]
    private MultiLeverDoor doorTarget;

    [SerializeField]
    private MeshRenderer mr;

    [SerializeField]
    private Light lightsrc;

    bool isOn;

    void Awake()
    {
        isOn = false;
        mr.material.color = Color.white;
        lightsrc.enabled = false;
    }

    public string GetItemName()
    {
        return "";
    }

    public void OnHover()
    {
        UIManager.instance.OnHover(isOn?"Switch off":"Switch On");
    }

    public void OnInteract(GameObject inventory)
    {
        isOn = !isOn;
        if(isOn) doorTarget.AddPower();
        else doorTarget.RemovePower();
        mr.material.color = isOn?Color.green:Color.white;
        lightsrc.enabled = isOn;
    }

    public void OnSighted()
    {
    }

    public void OnLookAway()
    {
    }

    public string GetDetails()
    {
        return "Lever";
    }
}
