using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class ItemSpawner : SManager
{
    [SerializeField] private List<ItemSettings> registeredItems;
    public PoolManager poolManager;

    public override void OnEnabled()
    {
        base.OnEnabled();
        foreach (var item in registeredItems)
        {
            RegisterItem(item);
        }
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        foreach (var item in registeredItems)
        {
            DeregisterItem(item);
        }
    }

    public void RegisterItem(ItemSettings itemSettings)
    {
        // Assuming GetPool() returns an existing pool or creates a new one
        itemSettings.itemPool = poolManager.GetPool(itemSettings.item);

        // Define the spawn action
        itemSettings.spawnAction = (position) =>
        {
            for (int i = 0; i < itemSettings.itemCount; i++)
            {
                // Spawn the item at the given position
                GameObject spawnedItem = itemSettings.itemPool.GetUnusedObject(false);
                Vector3 spawnPosition = position + Random.insideUnitSphere * Random.Range(0, itemSettings.randomSpread);
                spawnedItem.transform.position = spawnPosition;
                spawnedItem.SetActive(true);
            }
        };

        // Register the spawn action with the event
        itemSettings.spawnTriggerEvent.sharedEvent += itemSettings.spawnAction;
    }

    public void DeregisterItem(ItemSettings itemSettings)
    {
        // Deregister the spawn action from the event
        if (itemSettings.spawnAction != null)
        {
            itemSettings.spawnTriggerEvent.sharedEvent -= itemSettings.spawnAction;
        }
    }
}

[System.Serializable]
public class ItemSettings
{
    public GameObject item;
    public int itemCount;
    public float randomSpread;
    public SVector3Event spawnTriggerEvent;
    [HideInInspector] public PrefabPool itemPool;
    [HideInInspector] public Action<Vector3> spawnAction;
}