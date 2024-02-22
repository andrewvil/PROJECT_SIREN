using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TMP_Text hoverText;
    [SerializeField] private Image staminaBar;
    [SerializeField] private GameObject nightVisionUI;
    [SerializeField] private TMP_Text temperatureText;

    [SerializeField] private GameObject itemTipObject;
    [SerializeField] private ItemTip itemTip;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject discovIndex;

    [SerializeField] private GameObject NVScanner;  

    [SerializeField] private Volume vol;

    [SerializeField] private Image nvFill;

    
    [SerializeField] private GameObject cracked;
    [SerializeField] private GameObject cracked1;
    [SerializeField] private GameObject cracked2;

    [Header("Audio")]
    [SerializeField] private AudioSource ambientAC;
    [SerializeField] private AudioClip dead;

    

    private void Awake()
    {
        instance = this;
        Color col = staminaBar.color;
        col.a = 0f;
        staminaBar.color = col;
        nightVisionUI.SetActive(false);
        itemTipObject.SetActive(false);
    }

    public void OnHover(string text)
    {
        hoverText.text = text; // make it a prefab and put E key image
    }
    public void OnHoverExit()
    {
        hoverText.text = ""; 
    }

    public void SetStaminaLength(float lerpval)
    {
        Vector3 scale = Vector3.Lerp(new Vector3(0f,1f,1f), Vector3.one, lerpval);
        staminaBar.transform.localScale = scale;
    }

    void Update()
    {
        if(GameManager.instance.bGameOver) return;
        if(Input.GetKeyDown(KeyCode.R) && itemTipObject.activeInHierarchy)
        {
            itemTipObject.SetActive(false);

            if(fadeCoroutine!=null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
        }
    }

    public void SetCracked(int health)
    {
        if (health < 70)
            cracked.SetActive(true);
        else if (health > 70)
            cracked.SetActive(false);
        if (health < 40)
        {
            PlayerAudioController.instance.PlayAudio(AUDIOSOUND.CRITICAL);
            cracked1.SetActive(true);
        }
        else if (health > 40)
            cracked1.SetActive(false);
        if (health < 10)
            cracked2.SetActive(true);
        else if (health > 10)
            cracked2.SetActive(false);
    }

    public void SetStaminaAlpha(float lerpval)
    {
        float alpha = Mathf.SmoothStep(0f,1,lerpval);
        Color col = staminaBar.color;
        col.a = alpha;
        staminaBar.color = col;
    }

    public void ToggleNightVisionUI()
    {
        nightVisionUI.SetActive(!nightVisionUI.activeInHierarchy);
    }

    public void UpdateTemperatureText(float temp)
    {
        //Debug.Log("temp" + temp + " aa " + (temp - 60) / 30);
        nvFill.fillAmount = (temp - 60) / 30;
        temperatureText.text = Mathf.Round(temp*10)*0.1f + "C";
        temperatureText.color = Color.Lerp(Color.white,Color.red,(temp-60f)/(30f));
    }

    public void DisplayTip(string name, string desc, bool saveToIndex, bool isDanger = false)
    {
        if(saveToIndex)
        {
            if(playerData.discoveryIndex.Contains(name)) return;
            playerData.discoveryIndex.Add(name);
            playerData.discoveryIndex.Add(desc);
        }
        if(isDanger)
            PlayerAudioController.instance.PlayAudio(AUDIOSOUND.DANGER);
        itemTipObject.SetActive(true);

        if(fadeCoroutine == null)
            fadeCoroutine = StartCoroutine(FadeTip());
        
        itemTip.fTime_elapsed = 6f;
        itemTip.SetDetails(name,desc);
        
        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.ITEMTIP);
    }

    Coroutine fadeCoroutine;

    private IEnumerator FadeTip()
    {
        if(fadeCoroutine != null)yield break;
        for(float i = 0; i <= 0.9f; i+=0.1f)
        {
            itemTipObject.GetComponent<Image>().material.SetFloat("_Effect",i);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(5f);
        fadeCoroutine = null;
    }

    public void DisableAllPostProcessing()
    {
        if(vol.profile.TryGet<NoisePostProcess>(out NoisePostProcess noise))
        {
            noise.active = false;
        }
        NVScanner.SetActive(false);
    }

    public void OnDie()
    {
        if(vol.profile.TryGet<NightVisionPostProcess>(out NightVisionPostProcess nvpp))
        {
            nvpp.active = false;
        }
        if(vol.profile.TryGet<LensDistortion>(out LensDistortion ld))
        {
            ld.active = false;
        }
        ambientAC.Stop();
        ambientAC.clip = dead;
        ambientAC.Play();
    }

    
}
