using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "TimescaleManager", menuName = "SO/Managers/TimescaleManager")]
public class TimescaleManager : SManager
{
    public float timeScale = 1f;
    private float lastTimeScale = 1f;

    public override void OnEnabled()
    {
        base.OnEnabled();
        lastTimeScale = Time.timeScale;
    }

    public override void Update()
    {
        if (timeScale != lastTimeScale)
        {
            Time.timeScale = timeScale;
            lastTimeScale = timeScale;
        }
    }

    public void StartFreezeFrame(float duration)
    {
        handler.StartCoroutine(FreezeFrame(duration));
    }

    public IEnumerator FreezeFrame(float duration)
    {
        Time.timeScale = 0.01f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
}
