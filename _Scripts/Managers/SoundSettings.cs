using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
    public SEvent settingsLoaded, settingsChanged;
    public AudioMixer audioMixer;
    public SBool musicOn, SFXOn;
    public SFloat volume;

    private void OnEnable()
    {
        settingsLoaded.sharedEvent += ApplySettings;
    }

    private void OnDisable()
    {
        settingsLoaded.sharedEvent -= ApplySettings;
    }

    private void ApplySettings()
    {
        audioMixer.SetFloat("MasterVolume", -80 + (volume.Value * 80));
        audioMixer.SetFloat("MusicVolume", musicOn.Value ? 0 : -80);
        audioMixer.SetFloat("SFXVolume", SFXOn.Value ? 0 : -80);
    }
}
