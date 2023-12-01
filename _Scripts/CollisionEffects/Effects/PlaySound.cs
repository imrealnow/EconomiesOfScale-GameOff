using UnityEngine;

public class PlaySound : CollisionEffect
{
    public SoundManager soundManager;
    public AudioClip clip;
    public float volume = 1f;
    public float pitch = 1f;
    public bool increasePitchOnConsecutivePlays = false;

    private float lastPlayed = 0f;
    private int consecutivePlays = 0;
    private int maxConsecutivePlays = 5;
    public override void ApplyEffect(CollisionContext context)
    {
        float pitchToPlay = pitch;
        if (increasePitchOnConsecutivePlays)
        {
            if (Time.time - lastPlayed < 0.5f)
            {
                if (consecutivePlays < maxConsecutivePlays)
                    consecutivePlays++;
                pitchToPlay = Mathf.Pow(1.06f, consecutivePlays) * pitch;
            }
            else
            {
                consecutivePlays = 0;
            }
        }
        soundManager.PlaySFX(clip, context.point, volume, pitchToPlay);
        lastPlayed = Time.time;
    }
}
