using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundManager", menuName = "SO/Managers/SoundManager")]
public class SoundManager : SManager
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private GameObject audioSourcePrefab;
    [SerializeField] private PersistentScriptableObject audioSettings;
    [SerializeField] private SEvent settingsChanged;
    [SerializeField] private AudioClip musicLoop;
    [SerializeField] private bool playMusicOnStart = true;

    private PoolManager poolManager;
    private PoolObject musicSource;
    private AudioSource musicAudioSource;
    private PrefabPool audioSourcePool;

    public override void OnEnabled()
    {
        base.OnEnabled();
        poolManager = ManagerRegistry.Instance.GetManager<PoolManager>();
        audioSourcePool = poolManager.GetPool(audioSourcePrefab);
        audioSettings.LoadData();
        ApplySettings();
        if (settingsChanged != null)
            settingsChanged.sharedEvent += ApplySettings;
        if (musicLoop != null && playMusicOnStart)
            PlayMusic();
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        if (settingsChanged != null)
            settingsChanged.sharedEvent -= ApplySettings;

    }

    public void PlaySFX(AudioClip soundClip, Vector3 position, float volume = 1, float pitch = 1)
    {
        GameObject audioSourceObject = audioSourcePool.GetUnusedObject();
        audioSourceObject.transform.position = position;
        AudioSource audioSource = audioSourceObject.GetComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.outputAudioMixerGroup = sfxGroup;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();
        audioSourceObject.GetComponent<PoolObject>().ReturnToPool(soundClip.length);
    }

    public void PlaySFX(AudioClip soundClip)
    {
        PlaySFX(soundClip, Camera.main.transform.position);
    }

    public void PlayMusic()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.Play();
            return;
        }
        GameObject audioSourceObject = audioSourcePool.GetUnusedObject();
        if (Camera.main != null)
            audioSourceObject.transform.SetParent(Camera.main.transform);
        else
            audioSourceObject.transform.position = Vector3.zero;
        AudioSource audioSource = audioSourceObject.GetComponent<AudioSource>();
        musicSource = audioSourceObject.GetComponent<PoolObject>();
        musicAudioSource = audioSource;
        audioSource.clip = musicLoop;
        audioSource.outputAudioMixerGroup = musicGroup;
        audioSource.loop = true;
        audioSource.priority = 0;
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (musicAudioSource != null)
            musicAudioSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.GetComponent<AudioSource>().volume = volume;
    }

    private void ApplySettings()
    {
        float masterVolume;
        float musicVolume;
        float sfxVolume;
        mixer.SetFloat("MasterVolume", audioSettings.TryGetValue<float>("MasterVolume", out masterVolume) ? masterVolume : 0);
        mixer.SetFloat("MusicVolume", audioSettings.TryGetValue<float>("MusicVolume", out musicVolume) ? musicVolume : 0);
        mixer.SetFloat("SFXVolume", audioSettings.TryGetValue<float>("SFXVolume", out sfxVolume) ? sfxVolume : 0);
    }
}
