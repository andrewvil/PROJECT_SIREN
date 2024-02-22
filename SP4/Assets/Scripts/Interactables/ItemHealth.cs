using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealth : MonoBehaviour, IInteract, IPhotoObserver, ISightObserver
{
    public int health;

    public string GetItemName()
    {
        return "[E] Pick up";
    }

    public void OnHover()
    {
        UIManager.instance.OnHover(GetItemName());
    }

    public void OnInteract(GameObject inventory)
    {
        UIManager.instance.DisplayTip("First-Aid Kit", "Heals your wounds.", true);
        UIManager.instance.OnHoverExit();

        
        
        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.KEYCARD);

        Destroy(gameObject);    
    }

    public void OnLookAway()
    {
    }

    public void OnPhotoTaken()
    {
        UIManager.instance.DisplayTip("First-Aid Kit", "Heals your wounds.", true);
    }

    public void OnSighted()
    {

    }

    public string GetDetails()
    {
        return "First-Aid Kit";
    }
}
