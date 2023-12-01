using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageToken
{
    protected GameObject damageSource;
    protected float baseDamage;
    protected int stepCount;

    public float BaseDamage => baseDamage;
    public int StepCount => stepCount;

    public DamageToken(GameObject damageSource, float damage, int steps)
    {
        this.damageSource = damageSource;
        baseDamage = damage;
        stepCount = steps;
    }

    public virtual float GetDamage(float currentHealth)
    {
        if (stepCount <= 0)
            return 0;

        stepCount--;
        return baseDamage;
    }
}
