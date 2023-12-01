using UnityEngine;

[CreateAssetMenu(fileName = "ApplyKnockback", menuName = "CollisionEffect/ApplyKnockback")]
public class ApplyKnockback : CollisionEffect
{
    // default force
    public float knockbackForce = 10f;
    // flat force multiplier
    public float scaleForce = 1f;
    // damage to force curve
    public AnimationCurve forceScaleCurve;
    public RelativeTo relativeTo = RelativeTo.Velocity;

    public override void ApplyEffect(CollisionContext context)
    {
        ApplyEffect(context, knockbackForce);
    }

    public override void ApplyEffect(CollisionContext context, params object[] args)
    {
        float force = knockbackForce;
        if (args.Length > 0)
        {
            force = (float)args[0];
        }

        Vector3 direction;
        switch (relativeTo)
        {
            case RelativeTo.Point:
                direction = (context.collider.transform.position - context.point).normalized;
                break;
            case RelativeTo.Velocity:
                direction = context.relativeVelocity.normalized;
                break;
            case RelativeTo.Normal:
                direction = context.normal * -1f;
                break;
            default:
                direction = context.collider.transform.position;
                break;
        }
        Vector3 knockback = direction * forceScaleCurve.Evaluate(force) * scaleForce;
        Rigidbody2D otherRb = context.collider.GetComponent<Rigidbody2D>();
        if (otherRb != null)
        {
            otherRb.AddForce(knockback, ForceMode2D.Impulse);
        }
    }
}

[System.Serializable]
public enum RelativeTo
{
    Point,
    Velocity,
    Normal
}