public class DisableSelf : CollisionEffect
{
    public override void ApplyEffect(CollisionContext context)
    {
        context.collider.gameObject.SetActive(false);
    }
}
