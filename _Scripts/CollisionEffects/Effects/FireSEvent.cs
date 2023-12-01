using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollisionEffect", menuName = "CollisionEffect/FireSEvent", order = 1)]
public class FireSEvent : CollisionEffect
{
    public SEvent sEvent;
    public override void ApplyEffect(CollisionContext context)
    {
        if (sEvent != null)
            sEvent.Fire();
    }
}
