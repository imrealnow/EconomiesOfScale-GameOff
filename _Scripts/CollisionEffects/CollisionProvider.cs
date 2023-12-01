using System.Collections.Generic;
using UnityEngine;

public class CollisionProvider : MonoBehaviour, IPoolable
{
    [SerializeField] private CollisionEffectSet effectSet;
    [SerializeField] private bool collisionsActive = true;

    private List<GameObject> ignoredGameObjects = new List<GameObject>();

    public bool CollisionsActive { get => collisionsActive; set => collisionsActive = value; }
    public CollisionEffectSet EffectSet { get => effectSet; set => effectSet = value; }
    public List<GameObject> IgnoredGameObjects
    {
        get
        {
            if (ignoredGameObjects == null)
                ignoredGameObjects = new List<GameObject>();
            return ignoredGameObjects;
        }
        set => ignoredGameObjects = value;
    }

    public void AddEffect(CollisionEffect effect)
    {
        effectSet.effects.Add(effect);
    }

    public List<CollisionEffect> GetEffects()
    {
        return effectSet.effects;
    }

    public void IgnoreGameObject(GameObject go)
    {
        ignoredGameObjects.Add(go);
    }

    public void Reuse()
    {
        ignoredGameObjects = new List<GameObject>();
    }

    public void SetActive(bool active)
    {
        collisionsActive = active;
    }
}
