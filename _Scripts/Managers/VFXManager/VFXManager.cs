using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VFXManager", menuName = "SO/Managers/VFXManager")]
public class VFXManager : SManager
{
    public List<VFXSetting> vfxSettings = new List<VFXSetting>();
    public List<EventEffectSetting> eventEffects = new List<EventEffectSetting>();
    private Dictionary<GameObject, PrefabPool> vfxPools = new Dictionary<GameObject, PrefabPool>();

    private PoolManager poolManager;
    private PrefabPool vfxPool;

    public override void OnEnabled()
    {
        base.OnEnabled();
        poolManager = ManagerRegistry.Instance.GetManager<PoolManager>();
        InitialiseEffects();
    }

    public override void OnDisabled()
    {
        CleanEffects();
    }

    public void SpawnEffect(GameObject effect, Vector3 position)
    {
        AbstractPooledEffect pooledEffect = GetEffect(effect);
        if (pooledEffect != null)
        {
            pooledEffect.transform.position = position;
            pooledEffect.Play();
        }
    }

    public void SpawnEffect(GameObject effect, Vector3 position, Quaternion rotation)
    {
        AbstractPooledEffect pooledEffect = GetEffect(effect);
        if (pooledEffect != null)
        {
            pooledEffect.transform.position = position;
            pooledEffect.transform.rotation = rotation;
            pooledEffect.gameObject.SetActive(true);
            pooledEffect.Play();
        }
    }

    private AbstractPooledEffect GetEffect(GameObject effect)
    {
        PrefabPool effectPool;
        if (vfxPools.TryGetValue(effect, out effectPool))
        {
            GameObject effectObject = effectPool.GetUnusedObject();
            AbstractPooledEffect pooledEffect = effectObject.GetComponent<AbstractPooledEffect>();
            return pooledEffect;
        }
        else
        {
            PrefabPool prefabPool = poolManager.GetPool(effect);
            if (prefabPool != null)
            {
                vfxPools.Add(effect, prefabPool);
                return GetEffect(effect);
            }
        }
        return null;
    }

    private void InitialiseEffects()
    {
        foreach (VFXSetting setting in vfxSettings)
        {
            GameObject prefab = setting.vfx;
            PrefabPool pool = poolManager.GetPool(prefab);
            vfxPools.Add(prefab, pool);
            setting.Initialise(() =>
            {
                Vector3 position = setting.position.Value + setting.offset.Value;
                SpawnEffect(prefab, position);
            });
        }
        foreach (EventEffectSetting setting in eventEffects)
        {
            GameObject effect = setting.vfx;
            PrefabPool pool = poolManager.GetPool(effect);
            vfxPools.Add(effect, pool);
            setting.Initialise((pos) =>
            {
                Vector3 position = pos + setting.positionOffset;
                SpawnEffect(effect, position);
            });
        }
    }

    private void CleanEffects()
    {
        foreach (VFXSetting setting in vfxSettings)
        {
            setting.Clean();
            vfxPools.Remove(setting.vfx);
        }
        foreach (EventEffectSetting setting in eventEffects)
        {
            setting.Clean();
            vfxPools.Remove(setting.vfx);
        }
    }
}

[Serializable]
public struct VFXSetting
{
    public GameObject vfx;
    public Vector3Reference position;
    public Vector3Reference offset;
    public SEvent trigger;

    private Action spawnAction;

    public VFXSetting(GameObject vfx, Vector3Reference position, Vector3Reference offset, SEvent trigger)
    {
        this.vfx = vfx;
        this.position = position;
        this.offset = offset;
        this.trigger = trigger;
        this.spawnAction = null;
    }

    public void Initialise(Action spawnAction)
    {
        this.spawnAction = spawnAction;
        trigger.sharedEvent += spawnAction;
    }

    public void Clean()
    {
        trigger.sharedEvent -= spawnAction;
    }
}

[Serializable]
public struct EventEffectSetting
{
    public GameObject vfx;
    public SVector3Event triggerEvent;
    public Vector3 positionOffset;
    private Action<Vector3> spawnAction;

    public EventEffectSetting(GameObject vfx, SVector3Event triggerEvent, Action<Vector3> spawnAction, Vector3 positionOffset)
    {
        this.vfx = vfx;
        this.triggerEvent = triggerEvent;
        this.spawnAction = null;
        this.positionOffset = positionOffset;
    }

    public void Initialise(Action<Vector3> spawnAction)
    {
        this.spawnAction = spawnAction;
        triggerEvent.sharedEvent += spawnAction;
    }

    public void Clean()
    {
        triggerEvent.sharedEvent -= spawnAction;
    }
}