using UnityEngine;

public class PlaySFXSound : MonoBehaviour
{
    public SoundManager soundManager;
    public AudioClip defaultSound;
    public float volume = 1f;
    public float pitch = 1f;

    public void PlayDefaultSound()
    {
        soundManager.PlaySFX(defaultSound, transform.position, volume, pitch);
    }

    public void PlaySound(AudioClip sound)
    {
        soundManager.PlaySFX(sound, transform.position, volume, pitch);
    }
}
