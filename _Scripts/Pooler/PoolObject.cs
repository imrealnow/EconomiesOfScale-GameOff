using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public string OriginalName { set { _originalName = value; gameObject.name = value; } }
    public PrefabPool prefabPool;
    private string _originalName;

    protected virtual void OnDisable()
    {
        gameObject.name = _originalName + "(Not used)";
    }

    protected virtual void OnEnable()
    {
        gameObject.name = _originalName + "(Used)";
    }

    public void ReturnToPool(float seconds)
    {
        StartCoroutine(ReturnToPoolAfterSeconds(seconds));
    }

    IEnumerator ReturnToPoolAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}
