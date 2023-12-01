using UnityEngine;

[CreateAssetMenu(fileName = "CollisionEffect", menuName = "CollisionEffect/ChangeSInt", order = 1)]
public class ChangeSInt : CollisionEffect
{
    public SInt intVariable;
    public int amount;

    public override void ApplyEffect(CollisionContext context)
    {
        intVariable.Value += amount;
    }
}
