using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyTabletItem : MonoBehaviour, IItem
{

    [SerializeField] private List<GameObject> screens;
    private int screenIndex;

    [SerializeField] private Map mapScript;

    void Awake()
    {
        foreach(GameObject screen in screens)
        {
            screen.SetActive(false);
        }
        screenIndex = 0;
        screens[0].SetActive(true);
    }

    public void SwapScreen(bool front)
    {
        if(!canUse) return;
        screens[screenIndex].SetActive(false);
        if(front)
        {
            screenIndex = (screenIndex+1) % screens.Count;
        }
        else
        {
            screenIndex = (screenIndex+screens.Count-1) % screens.Count;
        }
        screens[screenIndex].SetActive(true);
    }
    
    public int GetItemID()
    {
        return 4;
    }

    public void GetRequiredControllers(GameObject obj, GameObject sightController)
    {
    }

    public bool IsItemInUse()
    {
        return mapScript.isUsing;


    }

    public void OnEMPOff()
    {
        canUse = true;
        screens[screenIndex].SetActive(true);
    }

    bool canUse = true;
    public void OnEMPTrigger()
    {
        canUse = false;
        screens[screenIndex].SetActive(false);
    }

    public void OnPrimaryAction()
    {

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

    public void RunBackgroundProcesses()
    {

    }

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        GameManager.instance.isInUI = true;
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.instance.isInUI = false;
    }
    

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
        transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + (Camera.main.transform.forward * 0.8f), Time.deltaTime * 25f);
    }
}
