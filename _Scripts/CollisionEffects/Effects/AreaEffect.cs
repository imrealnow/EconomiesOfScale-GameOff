using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollisionEffect", menuName = "CollisionEffect/AreaEffect", order = 1)]
public class AreaEffect : CollisionEffect
{
    public List<CollisionEffect> effectsToApply = new List<CollisionEffect>();
    public float radius;
    public int maxHits;
    public LayerMask layerMask;
    public LayerMask lineOfSightBlockingLayers;
    public bool requireLineOfSight;

    public override void ApplyEffect(CollisionContext context)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(layerMask);
        Collider2D[] results = new Collider2D[maxHits];
        int hitCount = Physics2D.OverlapCircle(context.point, radius, filter, results);
        for (int i = 0; i < hitCount; i++)
        {
            if (requireLineOfSight)
            {
                RaycastHit2D hit = Physics2D.Raycast(context.point, results[i].transform.position - context.point, radius, layerMask);
                if (hit.collider == null || hit.collider != results[i])
                {
                    int layer = results[i].gameObject.layer;
                    if (lineOfSightBlockingLayers == (lineOfSightBlockingLayers | (1 << layer)))
                        continue;
                }
            }
            Vector3 relativeVelocity = CollisionContext.GetRelativeVelocity(context.sourceCollider.gameObject, results[i].gameObject);
            Vector3 collisionNormal = (context.sourceCollider.transform.position - results[i].transform.position).normalized;
            foreach (CollisionEffect effect in effectsToApply)
            {
                effect.ApplyEffect(new CollisionContext(context.point, results[i], context.sourceCollider, relativeVelocity, collisionNormal));
            }
        }
    }
}
