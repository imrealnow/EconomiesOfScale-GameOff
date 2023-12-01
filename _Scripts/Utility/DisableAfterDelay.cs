using System.Collections;
using UnityEngine;

public class DisableAfterDelay : MonoBehaviour
{
    public GameObject objectToDisable;
    public void Disable(float delay)
    {
        StartCoroutine(DisableCoroutine(delay));
    }

    private IEnumerator DisableCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        objectToDisable.SetActive(false);
    }
}
