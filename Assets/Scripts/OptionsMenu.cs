using System;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Toggle fullscreenToggle;

    [Header("Mixers")]
    [SerializeField] AudioMixer mainMixer;

    [Header("Configurables")]
    [SerializeField] bool fullscreenState = false;
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        mainMixer.SetFloat("music", Mathf.Log10(volume)*20);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        mainMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
    }

    public void ToggleFullscreen()
    {
        if (fullscreenToggle.isOn)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }
}