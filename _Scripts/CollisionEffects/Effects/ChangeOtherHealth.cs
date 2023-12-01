using UnityEngine;

[CreateAssetMenu(fileName = "CollisionEffect", menuName = "CollisionEffect/ChangeOtherHealth", order = 1)]
public class ChangeOtherHealth : CollisionEffect
{
    public float amount;

    public override void ApplyEffect(CollisionContext context)
    {
        Health healthComponent = context.collider.GetComponent<Health>();
        healthComponent?.ChangeByAmount(amount);
    }

    public override void ApplyEffect(CollisionContext context, params object[] args)
    {
        float changeAmount = amount;
        if (args.Length > 0)
        {
            changeAmount = (float)args[0];
        }

        Health healthComponent = context.collider.GetComponent<Health>();
        healthComponent?.ChangeByAmount(-changeAmount);
    }
}
