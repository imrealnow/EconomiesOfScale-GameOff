using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticAberrationBumper : MonoBehaviour
{
    public AnimationCurve intensityCurve;
    public float defaultIntensity = 0;

    private ChromaticAberration effect;
    private Volume postProcessingVolume;

    private void Start()
    {
        postProcessingVolume = GetComponent<Volume>();
        postProcessingVolume.profile.TryGet(out effect);
    }

    public void BumpEffect(float duration)
    {
        StartCoroutine(Bump(duration));
    }

    private IEnumerator Bump(float duration)
    {
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            float progress = (endTime - Time.time) / duration;
            effect.intensity.value = intensityCurve.Evaluate(progress);
            yield return null;
        }
        effect.intensity.value = defaultIntensity;
    }
}
