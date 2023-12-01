using UnityEngine;

public class SpawnEffect : CollisionEffect
{
    public VFXManager effectsManager;
    public GameObject effect;
    public Vector2 offset;
    public bool rotateToNormal = false;

    public override void ApplyEffect(CollisionContext context)
    {
        Vector3 spawnPosition = context.point + (Vector3)offset;
        if (rotateToNormal)
            effectsManager.SpawnEffect(effect, spawnPosition, Quaternion.LookRotation(context.normal));
        else
            effectsManager.SpawnEffect(effect, spawnPosition);
    }
}
