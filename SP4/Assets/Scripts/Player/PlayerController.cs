using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour, IHealth
{
    [SerializeField]
    private List<GameObject> inventory;

    [SerializeField]
    private GameObject inventoryObject;

    [SerializeField]
    private GameObject sightController;

    [SerializeField]
    private GameObject model;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private Volume vol;

    [SerializeField]
    private GameObject status;

    [SerializeField]
    private PlayerData playerData;

    [SerializeField] private Volume damageVolume;

    private int equippedIndex;
    private FlashlightItem flashlight;

    #region adrenaline
    private Vignette vignette;
    private NightVisionPostProcess adrenalineColor;
    private LensDistortion adrenalineLens;
    public bool adrenalineOn;
    #endregion

    #region damagePP
    private Bloom damageBloom;
    #endregion

    void Start()
    {
        
        InitHealth();
        GetInventory();

        equippedIndex = 0;
        foreach(GameObject obj in inventory)
        {
            obj.SetActive(false);
        }
        inventory[equippedIndex].SetActive(true);
        
        //adrenaline PP
        vol.profile.TryGet<Vignette>(out vignette);
        vol.profile.TryGet<NightVisionPostProcess>(out adrenalineColor);
        vol.profile.TryGet<LensDistortion>(out adrenalineLens);

        adrenalineColor.active = false;
        adrenalineOn = false;
        //

        //damage PP
        damageVolume.profile.TryGet<Bloom>(out damageBloom);
    }

    void Update()
    {
        if(GameManager.instance.bGameOver) return;
        sightController.transform.position = transform.position;

        

        //weapon swapping
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            SwapItem(true, false);
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            SwapItem(false, false);
        }

        if(GameManager.instance.isInUI) return;
        

        if(canUse) {
            if(Input.GetMouseButtonDown(0))
            {
                ItemHealth healthItemCheck = inventory[equippedIndex].GetComponent<ItemHealth>();
                if(healthItemCheck != null)
                {
                    UpdateHealth(healthItemCheck.GetHealth());
                }

                inventory[equippedIndex].GetComponent<IItem>().OnPrimaryAction();

                if(inventory[equippedIndex].transform.parent == null)
                {
                    GetInventory();
                    SwapItem(false, true);
                }
            }
            if(Input.GetMouseButtonUp(0))
            {
                inventory[equippedIndex].GetComponent<IItem>().OnPrimaryActionRelease();
            }
            if(Input.GetMouseButtonDown(1))
            {
                inventory[equippedIndex].GetComponent<IItem>().OnSecondaryAction();
            }
            if(Input.GetMouseButtonUp(1))
        {
            inventory[equippedIndex].GetComponent<IItem>().OnSecondaryActionRelease();
        }
            if(Input.GetKeyDown(KeyCode.F))
            {
                flashlight.OnPrimaryAction();
            }
            foreach(GameObject obj in inventory)
            {
                if(obj.activeInHierarchy)
                    obj.GetComponent<IItem>()?.RunBackgroundProcesses();
            }
        }
        // foreach(GameObject obj in sightController.GetComponent<SightController>().GetObjectsInRange(15f))
        // {
        //     if(obj.GetComponent<EnemyBase>()!=null)
        //     {
        //         if(adrenalineCo==null)
        //             adrenalineCo = StartCoroutine(AdrenalineCoroutine());
        //         break;
        //     }
            
        // }

        //for testing only.
        if(Input.GetKeyDown(KeyCode.H))
        {
            UpdateHealth(-35);
        }
        HandleAdrenaline();

        FloorCheck();
        DoorCheck();
    }

    Coroutine adrenalineCo;
    Coroutine heartbeatCo;

    float bpm = 0f;
    private IEnumerator AdrenalineCoroutine()
    {
        cameraController.StartShake();
        adrenalineOn = true;
        bpm = 0.4f;
        if(heartbeatCo==null)
            heartbeatCo = StartCoroutine(HeartbeatCoroutine());
        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.ADRENALINE);

        adrenalineColor.active = true;

        yield return new WaitForSeconds(5f);
        adrenalineOn = false;
        adrenalineCo = null;
        yield return new WaitForSeconds(10f);

        adrenalineColor.active = false;

        cameraController.StopShake();

        if(heartbeatCo!=null) {
            StopCoroutine(heartbeatCo);
            heartbeatCo=null;
        }
    }

    private IEnumerator HeartbeatCoroutine()
    {
        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.HEARTBEAT);
        yield return new WaitForSeconds(bpm);
        heartbeatCo = StartCoroutine(HeartbeatCoroutine());
    }

    public void GetInventory()
    {
        inventory.Clear();
        playerData.inventoryIds.Clear();
        //get inventory
        foreach(Transform child in inventoryObject.transform)
        {
            IItem item = child.GetComponent<IItem>();
            item.GetRequiredControllers(gameObject, sightController);
            inventory.Add(child.gameObject);
            playerData.inventoryIds.Add(item.GetItemID());

            FlashlightItem fs = child.GetComponent<FlashlightItem>();
            if(fs!=null) flashlight=fs;
        }
    }

    void SwapItem(bool front, bool isConsumed)
    {
        if(!isConsumed)
        {
            if(inventory[equippedIndex].GetComponent<IItem>().IsItemInUse())
            {
                return;
            }
            inventory[equippedIndex].SetActive(false);
        }
        if(front)
        {
            equippedIndex = (equippedIndex + 1) % inventory.Count;
        }
        else
        {
            equippedIndex = (equippedIndex + inventory.Count - 1) % inventory.Count;
        }
        inventory[equippedIndex].SetActive(true);
        inventory[equippedIndex].transform.position = transform.position + Camera.main.transform.right;
    }

    [SerializeField]
    private int maxHealth = 100;

    private int health;

    public int GetHealth()
    {
        return health;
    }
    public void UpdateHealth(int amt)
    {
        if(GameManager.instance.bGameOver) return;
        health += amt;
        if(health > maxHealth) health = maxHealth;

        else if (health <= 0)
        {
            health = 0;
            Die();
            return;
        }

        //play adrenaline
        if(amt < 0)
        {
            PlayerAudioController.instance.PlayAudio(AUDIOSOUND.HURT);
            if(adrenalineCo==null)
                adrenalineCo = StartCoroutine(AdrenalineCoroutine());
        }
        UIManager.instance.SetCracked(health);
        damageBloom.intensity.value = (maxHealth - health) / (float)maxHealth * 2f;
    }

    void HandleAdrenaline()
    {
        if(adrenalineOn)
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.4f, 0.01f);
            adrenalineColor.blend.value = Mathf.Lerp(adrenalineColor.blend.value, 1f, 0.01f);
            cameraController.SetShakeMagnitude(Mathf.Lerp(cameraController.GetShakeMagnitude(),0.05f,0.01f));
            adrenalineLens.intensity.value = Mathf.Lerp(adrenalineLens.intensity.value,-0.4f,0.01f);
            adrenalineLens.scale.value = Mathf.Lerp(adrenalineLens.scale.value,0.9f,0.01f);
            
        }
        else
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.3f, 0.005f);
            adrenalineColor.blend.value = Mathf.Lerp(adrenalineColor.blend.value, -0.1f, 0.005f);
            cameraController.SetShakeMagnitude(Mathf.Lerp(cameraController.GetShakeMagnitude(),0f,0.005f));
            adrenalineLens.intensity.value = Mathf.Lerp(adrenalineLens.intensity.value,0f,0.005f);
            adrenalineLens.scale.value = Mathf.Lerp(adrenalineLens.scale.value,1f,0.005f);
            bpm = Mathf.Lerp(bpm, 0.7f, 0.005f);
        }
    }

    public void Die()
    {
        if(adrenalineCo != null)
        {
            adrenalineOn = false;
            StopCoroutine(adrenalineCo);
            adrenalineCo = null;
            adrenalineColor.active = false;
            cameraController.StopShake();
        }

        GameManager.instance.bGameOver = true;
        model.SetActive(false);

        //switch off adrenaline
        vignette.intensity.value = 0.3f;
        adrenalineColor.blend.value = 0f;
        cameraController.StopShake();
        adrenalineLens.intensity.value = 0f;
        adrenalineLens.scale.value = 0f;

        GetComponentsInChildren<MeshRenderer>()[0].enabled = false;
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        UIManager.instance.DisableAllPostProcessing();
        damageBloom.intensity.value = 0f;

        //jumpscare
        if(GameManager.instance.lastHitEnemy) {
            Camera.main.transform.parent = GameManager.instance.lastHitEnemy.transform;
            Camera.main.transform.localRotation = Quaternion.identity;
            Camera.main.transform.localPosition = Vector3.zero;
            PlayerAudioController.instance.PlayAudio(AUDIOSOUND.JUMPSCARE);
        }
        
        if(heartbeatCo != null)
        {
            StopCoroutine(heartbeatCo);
            heartbeatCo = null;
        }
        
        yield return new WaitForSeconds(1.25f);
        UIManager.instance.OnDie();
        GameManager.instance.Die();
    }

    public void InitHealth()
    {
        health = maxHealth;
    }

    bool canUse = true;
    Coroutine electronicsCoroutine;
    public void DisableEquipment()
    {
        canUse = false;
        status.SetActive(true);
        inventory[equippedIndex].GetComponent<IItem>()?.OnPrimaryActionRelease();
        inventory[equippedIndex].GetComponent<IItem>()?.OnSecondaryActionRelease();
        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.SYSTEMSDOWN);
        foreach(GameObject obj in inventory)
        {
            obj.GetComponent<IItem>()?.OnEMPTrigger();
        }
        if(electronicsCoroutine!=null) StopCoroutine(electronicsCoroutine);
        electronicsCoroutine = StartCoroutine(EnableElectronics());
    }

    private IEnumerator EnableElectronics()
    {
        yield return new WaitForSeconds(5f);

        foreach(GameObject obj in inventory)
        {
            obj.GetComponent<IItem>()?.OnEMPOff();
        }
        status.SetActive(false);
        canUse = true;
    }

    public void FloorCheck()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
        {
            if (hit.collider.gameObject.layer == 7)
            {
                if (hit.collider.gameObject.CompareTag("Floor"))
                {
                    hit.collider.gameObject.layer = 8;
                }
            }
        }
    }

    public void DoorCheck()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.green);
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 5f))
        {
            if (hit.collider.gameObject.layer == 7)
            {
                if (hit.collider.gameObject.CompareTag("Door"))
                {
                    hit.collider.gameObject.layer = 8;
                }
            }
        }
    }
}
