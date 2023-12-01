using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPooledEffect : PoolObject
{
    [SerializeField]
    protected float duration = 1.0f;
    public abstract void Play();
}
