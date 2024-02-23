using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealth : MonoBehaviour, IInteract, IPhotoObserver, ISightObserver, IItem
{
    private int health;
    public int GetHealth()
    {
        return health;
    }

    public bool isDropped;

    [SerializeField]
    private Rigidbody rb;

    void Awake()
    {
        isDropped = true;
        health = 35;
    }

    public string GetItemName()
    {
        return "[E] Pick up";
    }

    public void OnHover()
    {
        UIManager.instance.OnHover(GetItemName());
    }

    void OnEnable()
    {
        //inventory parent
        if(transform.parent != null)
        {
            PlayerAudioController.instance.PlayAudio(AUDIOSOUND.MEDKIT);
        }
    }

    public void OnInteract(GameObject inventory)
    {
        UIManager.instance.DisplayTip("First-Aid Kit", "Heals your wounds.", true);
        UIManager.instance.OnHoverExit();
        isDropped = false;
        transform.parent = inventory.transform;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.MEDKIT);
        gameObject.layer = 2;
        gameObject.SetActive(false);   
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

    void Update()
    {
        if(!isDropped)
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + (Camera.main.transform.forward - (Camera.main.transform.up * 0.5f) + (Camera.main.transform.right * 0.2f)) * 0.8f, Time.deltaTime * 25f);
        }
    }

    public int GetItemID()
    {
        return 5;
    }

    public void OnPrimaryAction()
    {
        gameObject.transform.parent = null;
        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.HEAL);
        Destroy(gameObject);
    }

    public void OnPrimaryActionRelease()
    {
    }

    public void OnSecondaryAction()
    {
    }

    public void OnSecondaryActionRelease()
    {
    }

    public bool IsItemInUse()
    {
        return false;
    }

    public void RunBackgroundProcesses()
    {
    }

    public void OnEMPTrigger()
    {
    }

    public void OnEMPOff()
    {
    }

    public void GetRequiredControllers(GameObject obj, GameObject sightController)
    {
    }
}
