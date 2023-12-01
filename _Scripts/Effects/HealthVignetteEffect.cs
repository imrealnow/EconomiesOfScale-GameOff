using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthVignetteEffect : MonoBehaviour
{
    public SFloat playerHealth;
    public float maxHealth = 100f;
    public float minIntensity = 0.1f;
    public float maxIntensity = 0.3f;
    public AnimationCurve intensityCurve;

    private Volume postProcessingVolume;
    private Vignette effect;

    private void Start()
    {
        postProcessingVolume = GetComponent<Volume>();
        postProcessingVolume.profile.TryGet(out effect);
    }

    private void FixedUpdate()
    {
        UpdateVignette();
    }

    private void UpdateVignette()
    {
        if (effect != null)
        {
            float intensity = intensityCurve.Evaluate(playerHealth.Value / maxHealth);
            effect.intensity.value = Mathf.Lerp(maxIntensity, minIntensity, intensity);
        }
        else
        {
            postProcessingVolume.profile.TryGet(out effect);
        }
    }
}

