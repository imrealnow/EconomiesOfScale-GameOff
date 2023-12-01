using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class PooledVFXEffect : AbstractPooledEffect
{
    private VisualEffect visualEffect;
    
    void Awake()
    {
        visualEffect = GetComponent<VisualEffect>();
    }
    
    public override void Play()
    {
        StartCoroutine(PlayEffect());
    }
    
    private IEnumerator PlayEffect()
    {
        visualEffect.Play();
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
