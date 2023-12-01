public class FireSVector3Event : CollisionEffect
{
    public SVector3Event sVector3Event;

    public override void ApplyEffect(CollisionContext context)
    {
        sVector3Event?.Fire(context.point);
    }
}
