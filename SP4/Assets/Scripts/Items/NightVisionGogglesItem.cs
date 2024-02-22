using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NightVisionGogglesItem : MonoBehaviour, IItem
{
    [SerializeField] private Volume globalVolume;
    [SerializeField] private GameObject model;
    [SerializeField] float fTime_elapsed;
    [SerializeField] private GameObject NVScanner;

    [Header("Night Vision Enemy")]
    [SerializeField] private GameObject nvEnemyPrefab;
    [SerializeField] private float spawnChance;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float distanceDeviation;
    [SerializeField] private LayerMask spawnLayer;

    [Header("Audio")]
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip onSfx;
    [SerializeField] private AudioClip offSfx;
    [SerializeField] private AudioClip equipSfx;

    [SerializeField] private Light NVLight;

    private bool isOn;
    private GameObject spawnedEnemy;
    public float temperature;
    private Coroutine toggleCoroutine;

    void OnEnable()
    {
        src.PlayOneShot(equipSfx);
    }

    void Start()
    {
        fTime_elapsed = 2f;
        isOn = false;
        GameManager.instance.nvIsOn = isOn;
        temperature = 49.0f;
    }

    public int GetItemID()
    {
        return 1;
    }

    public void GetRequiredControllers(GameObject obj, GameObject sightController)
    {

    }

    public void OnPrimaryAction()
    {
        if(fTime_elapsed > 1f)
        {
            if(!isOn && temperature > 70f) return;
            fTime_elapsed = 0f;
            isOn = !isOn;
            GameManager.instance.nvIsOn = isOn;
            toggleCoroutine = StartCoroutine(ToggleNightVision());
        }
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

    private IEnumerator ToggleNightVision()
    {
        if(isOn)
        {
            src.PlayOneShot(onSfx);
            yield return new WaitForSeconds(0.5f);
            if(!isOn) yield break;
            if(GameManager.instance.bGameOver) yield break;
            model.SetActive(false);
            if(globalVolume.profile.TryGet<NightVisionPostProcess>(out NightVisionPostProcess com))
            {
                com.active = true;
            }
            if(globalVolume.profile.TryGet<LensDistortion>(out LensDistortion ld))
            {
                ld.active = true;
            }
            NVScanner.SetActive(true);
            NVLight.enabled = true;
            UIManager.instance.ToggleNightVisionUI();
            if (RandomNVEnemy())
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, transform.forward, out hit, distanceFromPlayer))
                {
                    SpawnNVEnemy(hit.point);
                }
                else
                {
                    SpawnNVEnemy(transform.position + Camera.main.transform.forward * distanceFromPlayer);
                }
            }
        }
        else
        {
            isOn = true;
            GameManager.instance.nvIsOn = isOn;
            src.PlayOneShot(offSfx);
            yield return new WaitForSeconds(0.5f);
            if(GameManager.instance.bGameOver) yield break;
            if(globalVolume.profile.TryGet<NightVisionPostProcess>(out NightVisionPostProcess com))
            {
                com.active = false;
            }
            if(globalVolume.profile.TryGet<LensDistortion>(out LensDistortion ld))
            {
                ld.active = false;
            }
            model.SetActive(true);
            NVScanner.SetActive(false);
            NVLight.enabled = false;
            UIManager.instance.ToggleNightVisionUI();
            isOn = false;
            GameManager.instance.nvIsOn = isOn;
            if(temperature > 80f) temperature = 80f;
            if (spawnedEnemy)
                Destroy(spawnedEnemy);
            yield return new WaitForSeconds(0.5f);
        }
        toggleCoroutine = null;
    }

    void Update()
    {
        if(GameManager.instance.bGameOver) return;
        if(isOn)
        {
            if(temperature > 90f && toggleCoroutine == null) 
            {
                //TODO: overheat
                fTime_elapsed = 0f;
                isOn = false;
                GameManager.instance.nvIsOn = isOn;
                toggleCoroutine = StartCoroutine(ToggleNightVision());
            }
            else
            {
                temperature+=Time.deltaTime*2f;
                UIManager.instance.UpdateTemperatureText(temperature);
            }
        }
        else
        {
            temperature-=Time.deltaTime;
            if(temperature < 60f) temperature = 60f;
        }
        fTime_elapsed += Time.deltaTime;
        if(globalVolume.profile.TryGet<NoisePostProcess>(out NoisePostProcess com))
        {
            if(fTime_elapsed <= 0.5f)
            {
                com.blend.value = Mathf.SmoothStep(0f,1f,fTime_elapsed * 2f);
            }
            else if (fTime_elapsed <= 1f)
            {
                com.blend.value = Mathf.SmoothStep(1f,0f,(fTime_elapsed-0.5f) * 4f);
            }
            else
            {
                com.blend.value = 0f;
            }
        }
        transform.LookAt(transform.position - Camera.main.transform.forward);
        if(isOn)
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position, Time.deltaTime * 5f);
        else
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + (Camera.main.transform.forward + (Camera.main.transform.right - Camera.main.transform.up) * 0.5f) * 0.5f, Time.deltaTime * 15f);
    }

    public bool IsItemInUse()
    {
        return isOn || toggleCoroutine != null;
    }

    private bool RandomNVEnemy()
    {
        int rng = Random.Range(0, 100);
        Debug.Log("RNG: " + rng);
        if (rng < spawnChance)
            return true;

        return false;
    }

    private void SpawnNVEnemy(Vector3 spawnPosition)
    {
        bool spawned = false;
        float x;
        float z;
        RaycastHit hit;

        x = spawnPosition.x + Random.Range(-distanceDeviation, distanceDeviation);
        z = spawnPosition.z + Random.Range(-distanceDeviation, distanceDeviation);

        if (Physics.Raycast(new Vector3(x, 1f, z), Vector3.down, out hit, 1.2f, spawnLayer))
        {
            spawnedEnemy = Instantiate(nvEnemyPrefab, hit.point, Quaternion.identity);
            spawnedEnemy.GetComponent<EnemyAnti>().SetTarget(transform.parent.parent);
            spawned = true;
        }

        if (!spawned)
        {
            Debug.Log("Retry spawn");
            SpawnNVEnemy(spawnPosition);
        }
    }

    public void RunBackgroundProcesses()
    {
        temperature -= Time.deltaTime;
    }

    public void OnEMPTrigger()
    {
        if(isOn)
        {
            OnPrimaryAction();
        }
    }

    public void OnEMPOff()
    {
        
    }
}
