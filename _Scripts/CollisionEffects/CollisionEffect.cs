using UnityEngine;


[System.Serializable]
public abstract class CollisionEffect : ScriptableObject
{
    public CollisionEffect() { }

    public abstract void ApplyEffect(CollisionContext context);

    public virtual void ApplyEffect(CollisionContext context, params object[] args)
    {
        // allow for extra arguments to be passed in
        ApplyEffect(context);
    }
}

[System.Serializable]
public struct CollisionContext
{
    public Vector3 point;
    public Collider2D collider;
    public Collider2D sourceCollider;
    public Vector3 relativeVelocity;
    public Vector3 normal;

    public CollisionContext(
        Vector3 point,
        Collider2D collider,
        Collider2D sourceCollider,
        Vector3 relativeVelocity,
        Vector3 normal)
    {
        this.point = point;
        this.collider = collider;
        this.sourceCollider = sourceCollider;
        this.relativeVelocity = relativeVelocity;
        this.normal = normal;
    }

    public static Vector3 GetRelativeVelocity(GameObject first, GameObject second)
    {
        Rigidbody2D firstRigidbody = first.GetComponent<Rigidbody2D>();
        Rigidbody2D secondRigidbody = second.GetComponent<Rigidbody2D>();
        Vector3 relativeVelocity = Vector3.zero;
        if (firstRigidbody != null)
            relativeVelocity = firstRigidbody.velocity;
        if (secondRigidbody != null)
            relativeVelocity -= (Vector3)secondRigidbody.velocity;
        return relativeVelocity;
    }
}