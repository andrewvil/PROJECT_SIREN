using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraItem : MonoBehaviour, IItem
{
    private bool isAiming;

    [SerializeField]
    private Material screenMaterial;

    [SerializeField]
    private Material whiteMaterial;

    [SerializeField]
    private Material blackMaterial;

    [SerializeField]
    private MeshRenderer mr;

    [SerializeField]
    private SightController sc;

    [SerializeField]
    private Light lightSource;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip picClip;

    [SerializeField]
    private AudioClip equipClip;

    [SerializeField]
    private Image flashOff;

    private float cd;

    void OnEnable()
    {
        audioSource.PlayOneShot(equipClip);
    }

    void Start()
    {
        cd = 0f;
        isAiming = false;
        lightSource.enabled = false;
        flashOff.enabled = false;
    }

    public int GetItemID()
    {
        return 0;
    }

    public void OnPrimaryAction()
    {
        if(isAiming && cd <= 0f)
        {
            StartCoroutine(TakePhoto());
        }
    }

    public void OnPrimaryActionRelease()
    {
    }

    public void OnSecondaryAction()
    {
        isAiming = true;
    }

    public void OnSecondaryActionRelease()
    {
        isAiming = false;
    }

    void Update()
    {
        if(cd > 0f)
        {
            flashOff.enabled = true;
            cd -= Time.deltaTime;
        }
        else
            flashOff.enabled = false;
        transform.LookAt(transform.position - Camera.main.transform.forward);
        if(isAiming)
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + Camera.main.transform.forward * 0.1f, Time.deltaTime * 50f);
        else
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + (Camera.main.transform.forward + (Camera.main.transform.right - Camera.main.transform.up) * 0.5f) * 0.5f, Time.deltaTime * 25f);
    }

    private IEnumerator TakePhoto()
    {
        cd = 2f;
        mr.material = whiteMaterial;
        lightSource.enabled = true;
        audioSource.PlayOneShot(picClip);
        yield return new WaitForSeconds(0.1f);

        //alert objects
        foreach(GameObject obj in sc.GetObjectsInRange(15f))
        {
            obj.GetComponent<IPhotoObserver>()?.OnPhotoTaken();
        }

        mr.material = screenMaterial;
        lightSource.enabled = false;
    }

    public void GetRequiredControllers(GameObject obj, GameObject sightController)
    {
        sc = sightController.GetComponent<SightController>();
    }

    public bool IsItemInUse()
    {
        return isAiming;
    }

    public void RunBackgroundProcesses()
    {

    }

    public void OnEMPTrigger()
    {
        mr.material = blackMaterial;
    }

    public void OnEMPOff()
    {
        mr.material = screenMaterial;
    }

    public bool IsActionLoud() => true;
}
