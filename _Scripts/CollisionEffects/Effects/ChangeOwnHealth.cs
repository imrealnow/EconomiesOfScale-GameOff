using UnityEngine;

[CreateAssetMenu(fileName = "ChangeOwnHealth", menuName = "CollisionEffect/ChangeOwnHealth", order = 1)]
public class ChangeOwnHealth : CollisionEffect
{
    public float amount;

    public override void ApplyEffect(CollisionContext context)
    {
        Health healthComponent = context.sourceCollider.GetComponent<Health>();
        healthComponent?.ChangeByAmount(amount);
    }
}
