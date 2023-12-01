using UnityEngine;

public class PooledAudioSource : PoolObject
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        audioSource.Stop();
        audioSource.pitch = 1f;
        audioSource.volume = 1f;
        audioSource.clip = null;
        audioSource.priority = 128;
    }
}
