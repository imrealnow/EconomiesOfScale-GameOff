using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PooledParticleEffect : AbstractPooledEffect
{
    private new ParticleSystem particleSystem;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public override void Play()
    {
        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        particleSystem.Play();
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
