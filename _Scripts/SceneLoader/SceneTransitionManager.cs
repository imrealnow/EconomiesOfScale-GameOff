using System.Collections;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Attach the CanvasGroup in the inspector.

    // The time it takes for the scene to fade in and out.
    public float transitionTime = 1f;

    public AnimationCurve transitionCurve; // Adjust this in the inspector.

    public SEvent sceneLoaded;
    public SEvent sceneUnloaded;

    private void OnEnable()
    {
        sceneLoaded.sharedEvent += OnSceneLoad;
        sceneUnloaded.sharedEvent += OnSceneUnload;
    }

    private void OnDisable()
    {
        sceneLoaded.sharedEvent -= OnSceneLoad;
        sceneUnloaded.sharedEvent -= OnSceneUnload;
    }

    private void OnSceneLoad()
    {
        StartCoroutine(FadeIn());
    }

    private void OnSceneUnload()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < transitionTime)
        {
            time += Time.deltaTime;
            float curveTime = time / transitionTime;
            canvasGroup.alpha = 1f - transitionCurve.Evaluate(curveTime); // Use curve for transition.
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private IEnumerator FadeOut()
    {
        float time = 0f;
        while (time < transitionTime)
        {
            time += Time.deltaTime;
            float curveTime = time / transitionTime;
            canvasGroup.alpha = transitionCurve.Evaluate(curveTime); // Use curve for transition.
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}