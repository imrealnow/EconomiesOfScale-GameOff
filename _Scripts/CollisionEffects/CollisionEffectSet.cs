using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollisionEffectSet", menuName = "SO/CollisionEffects/CollisionEffectSet", order = 1)]
public class CollisionEffectSet : ScriptableObject
{
    public List<CollisionEffect> effects = new List<CollisionEffect>();
}
