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
    
    public void SetMasterVolume()
    {
        mixer.SetFloat("MasterVolume", masterSlider.value);
    }
    public void SetSFXVolume()
    {
        mixer.SetFloat("SFXVolume", sfxSlider.value);
    }
    public void SetBGMVolume()
    {
        mixer.SetFloat("BGMVolume", bgmSlider.value);
    }
    public void SetBrightVolume()
    {
        if (volume.profile.TryGet(out NoisePostProcess bright))
            bright.brightness.value = brightSlider.value;
    }
}
