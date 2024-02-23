using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour, IItem, ISightObserver, IPhotoObserver, IInteract
{
    [SerializeField]
    private List<GameObject> objectsInRange;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private AudioSource src;

    [SerializeField]
    private AudioClip pulseClip;

    [SerializeField]
    private AudioClip clank;

    [SerializeField]
    private AudioClip equip;

    [SerializeField]
    private Collider rangeCollider;

    [SerializeField] private List<GameObject> holograms;

    public bool isDroppped;

    void Awake()
    {
        isDroppped = true;
        rangeCollider.enabled = false;

        foreach(GameObject hologram in holograms)
        {
            hologram.SetActive(false);
        }
    }

    void OnEnable()
    {
        src.PlayOneShot(equip);
    }

    public int GetItemID()
    {
        return 2;
    }

    public void GetRequiredControllers(GameObject obj, GameObject sightController)
    {

    }

    public void OnPrimaryAction()
    {
        StartCoroutine(Throw());
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

    void Update()
    {
        if(GameManager.instance.bGameOver) return;
        if(transform.parent != null && !isDroppped)
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + (Camera.main.transform.forward + (Camera.main.transform.right - Camera.main.transform.up) * 0.5f) * 0.5f, Time.deltaTime * 25f);
        }
    }

    int pulses = 5;
    private IEnumerator Throw()
    {
        rb.constraints = RigidbodyConstraints.None;
        transform.parent = null;
        rb.AddForce(Camera.main.transform.forward * 15f + Camera.main.transform.up, ForceMode.Impulse);
        rangeCollider.enabled = true;
        yield return new WaitForSeconds(5);

        //activate all holograms
        foreach(GameObject hologram in holograms)
        {
            hologram.SetActive(true);
        }
        
        rb.constraints = RigidbodyConstraints.FreezeAll;
        StartCoroutine(Pulse());
    }

    private IEnumerator Pulse()
    {
        if(GameManager.instance.bGameOver) yield break;
        if(pulses <= 0)
        {
            Destroy(gameObject);
        }
        pulses--;
        src.PlayOneShot(pulseClip);
        foreach(GameObject obj in objectsInRange)
        {
            obj.GetComponent<IAudioObserver>().Notify(transform.position,gameObject);
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(Pulse());
    }

    void OnTriggerEnter(Collider other)
    {
        IAudioObserver iao = other.gameObject.GetComponent<IAudioObserver>();
        if(iao != null)
        {
            objectsInRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(objectsInRange.Contains(other.gameObject))
        {
            objectsInRange.Remove(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(transform.parent == null && !isDroppped)
            src.PlayOneShot(clank);
    }

    public bool IsItemInUse()
    {
        return false;
    }

    public void OnPhotoTaken()
    {
        UIManager.instance.DisplayTip("Decoy Bomb", "Red herring for threats.", true);
    }

    public void OnSighted()
    {
    }

    public void OnLookAway()
    {
    }

    public void RunBackgroundProcesses()
    {

    }

    public void OnEMPTrigger()
    {
        if(transform.parent == null && !isDroppped)
            Destroy(gameObject);
    }

    public void OnEMPOff()
    {
        
    }

    public string GetItemName()
    {
        return "Decoy grenade";
    }

    public void OnHover()
    {
        if(isDroppped)
            UIManager.instance.OnHover("[E] Pick up");
    }

    public void OnInteract(GameObject inventory)
    {
        if(isDroppped)
        {
            transform.parent = inventory.transform;
            isDroppped = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            PlayerAudioController.instance.PlayAudio(AUDIOSOUND.DECOYEQUIP);
            gameObject.SetActive(false);
        }
    }

    public string GetDetails()
    {
        return "Decoy grenade";
    }
}
