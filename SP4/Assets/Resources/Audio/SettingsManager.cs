using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider bgmSlider;

    [SerializeField] private Volume volume;
    [SerializeField] private Slider brightSlider;
    [SerializeField] private Slider mouseSlider;
    [SerializeField] private CameraController cc;

    private void Awake()
    {
        masterSlider.value = PlayerPrefsManager.Load("MasterVolume");
        sfxSlider.value = PlayerPrefsManager.Load("SFXVolume");
        bgmSlider.value = PlayerPrefsManager.Load("BGMVolume");
        brightSlider.value = PlayerPrefsManager.Load("BrightVolume");
        mouseSlider.value = PlayerPrefsManager.Load("MouseSensitivity");
    }
    public void SetMasterVolume()
    {
        mixer.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefsManager.Save("MasterVolume", masterSlider.value);
    }
    public void SetSFXVolume()
    {
        mixer.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefsManager.Save("SFXVolume", sfxSlider.value);
    }
    public void SetBGMVolume()
    {
        mixer.SetFloat("BGMVolume", bgmSlider.value);
        PlayerPrefsManager.Save("BGMVolume", bgmSlider.value);
    }
    public void SetBrightVolume()
    {
        if (volume.profile.TryGet(out NoisePostProcess bright))
        {
            bright.brightness.value = brightSlider.value;
            PlayerPrefsManager.Save("BrightVolume", brightSlider.value);
        }
    }

    public void SetMouseSense()
    {
        cc.mouseSensitivity = mouseSlider.value;
        PlayerPrefsManager.Save("MouseSensitivity", mouseSlider.value);
    }
}
