using System.Collections;
using UnityEngine;

public class DelayedSEventFirer : MonoBehaviour
{
    public float delay;
    public SEvent sEvent;

    public void FireEvent()
    {
        StartCoroutine(FireEventCoroutine());
    }

    private IEnumerator FireEventCoroutine()
    {
        yield return new WaitForSeconds(delay);
        sEvent.Fire();
    }
}
