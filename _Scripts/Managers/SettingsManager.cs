using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

[CreateAssetMenu(fileName = "SettingsManager", menuName = "SO/Managers/SettingsManager", order = 1)]
public class SettingsManager : SManager
{
    public SEvent settingsChanged, settingsLoaded;

    public SBool musicOn, SFXOn;
    public SFloat volume;

    private string path;

    public override void OnEnabled()
    {
        base.OnEnabled();
        path = Application.persistentDataPath + "/Settings.xml";
        if (settingsChanged != null)
            settingsChanged.sharedEvent += SaveSettings;
        LoadSettings();
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        if (settingsChanged != null)
            settingsChanged.sharedEvent -= SaveSettings;
        SaveSettings();
    }

    private void SaveSettings()
    {
        SettingsContainer settings = new SettingsContainer(musicOn.Value, SFXOn.Value, volume.Value);
        XmlSerializer serializer = new XmlSerializer(typeof(SettingsContainer));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, settings);
        }
        settingsLoaded.Fire();
    }

    private void LoadSettings()
    {
        SettingsContainer savedfile;
        if (!File.Exists(path))
        {
            savedfile = new SettingsContainer();
        }
        else
        {
            var serializer = new XmlSerializer(typeof(SettingsContainer));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                savedfile = serializer.Deserialize(stream) as SettingsContainer;
            }
        }

        musicOn.Value = savedfile.MusicOn;
        SFXOn.Value = savedfile.SFXOn;
        volume.Value = savedfile.Volume;
        settingsLoaded.Fire();
    }
}

[Serializable]
public class SettingsContainer
{
    public bool MusicOn;
    public bool SFXOn;
    public float Volume;

    public SettingsContainer(bool musicOn, bool SFXOn, float volume)
    {
        MusicOn = musicOn;
        this.SFXOn = SFXOn;
        Volume = volume;
    }

    public SettingsContainer()
    {
        MusicOn = true;
        SFXOn = true;
        Volume = 1f;
    }
}


