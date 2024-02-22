using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour, IInteract
{
    public string GetItemName()
    {
        return null;
    }

    public void OnHover()
    {
        UIManager.instance.OnHover("Escape");
    }

    public void OnInteract(GameObject inventory)
    {
        GameManager.instance.Escape();
    }
}
