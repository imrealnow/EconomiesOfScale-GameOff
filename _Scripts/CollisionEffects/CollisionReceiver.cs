using System.Collections.Generic;
using UnityEngine;

public class CollisionReceiver : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D otherCollider)
    {
        EvaluateEffects(otherCollider);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "interactor") return;
        if (collision.otherCollider.GetComponent<CollisionIgnorer>() != null) return;
        if (collision.collider.GetComponent<CollisionIgnorer>() != null) return;
        EvaluateEffects(collision.collider);
    }

    private void EvaluateEffects(Collider2D otherCollider)
    {
        CollisionProvider collisionEffector = otherCollider.gameObject.GetComponent<CollisionProvider>();
        if (collisionEffector != null && collisionEffector.gameObject.activeInHierarchy)
        {
            if (collisionEffector.IgnoredGameObjects.Contains(gameObject)) return;
            Vector3 relativeVelocity = CollisionContext.GetRelativeVelocity(gameObject, otherCollider.gameObject);
            Vector3 collisionNormal = (transform.position - otherCollider.transform.position).normalized;
            List<CollisionEffect> effects = collisionEffector.GetEffects();
            if (effects != null && effects.Count > 0)
            {
                foreach (CollisionEffect effect in collisionEffector.GetEffects())
                {

                    effect.ApplyEffect(new CollisionContext(transform.position, otherCollider, GetComponent<Collider2D>(), relativeVelocity, collisionNormal));
                }
            }
        }
    }
}
