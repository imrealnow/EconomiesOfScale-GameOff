using UnityEngine;

public abstract class PooledProjectile : PoolObject
{
    [SerializeField] protected CollisionProvider collisionProvider;
    protected GameObject projectileSource;

    public void SetProjectileSource(GameObject source)
    {
        projectileSource = source;
        collisionProvider.IgnoreGameObject(source);
    }

    protected abstract RaycastHit2D CheckCollision();

    protected Vector3 GetRelativeProjectileVelocity(RaycastHit2D collision, Vector3 projectileVelocity)
    {
        Vector3 relativeVelocity = projectileVelocity;
        Rigidbody2D otherRigidbody = collision.collider.GetComponent<Rigidbody2D>();
        if (otherRigidbody != null)
            relativeVelocity -= (Vector3)otherRigidbody.velocity;
        return relativeVelocity;
    }

    protected virtual void ApplyEffects(CollisionContext context)
    {
        if (collisionProvider.CollisionsActive)
        {
            foreach (CollisionEffect effect in collisionProvider.GetEffects())
            {
                effect.ApplyEffect(context);
            }
        }
    }

    protected bool ValidateCollision(CollisionContext context)
    {
        if (context.collider == null) return false;
        if (context.collider.gameObject == projectileSource) return false;
        if (context.collider.gameObject.layer == LayerMask.NameToLayer("Interactor")) return false;
        if (collisionProvider.IgnoredGameObjects.Contains(context.sourceCollider.gameObject)) return false;
        return true;
    }
}
