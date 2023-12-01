using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolManager", menuName = "SO/Managers/PoolManager")]
public class PoolManager : SManager
{
    public int defaultPoolSize = 20;
    public bool expandableByDefault = true;
    public List<PrefabToPool> prefabsToPool = new List<PrefabToPool>();
    public SEvent resetEvent;

    private Dictionary<GameObject, PrefabPool> poolDictionary = new Dictionary<GameObject, PrefabPool>();
    private GameObject poolContainer;

    public override void OnEnabled()
    {
        base.OnEnabled();
        resetEvent.sharedEvent += Reset;
        poolContainer = PoolContainerSingleton.Instance.gameObject;
        if (poolContainer == null)
        {
            poolContainer = handler.gameObject;
        }
        foreach (PrefabToPool prefab in prefabsToPool)
        {
            PrefabPool pool = new PrefabPool(prefab.prefab, prefab.size, this, expandableByDefault, poolContainer);
            poolDictionary.Add(prefab.prefab, pool);
        }
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        resetEvent.sharedEvent -= Reset;
    }

    private void Reset()
    {
        for (int i = 0; i < poolContainer.transform.childCount; i++)
        {
            Transform child = poolContainer.transform.GetChild(i);
            for (int j = 0; j < child.childCount; j++)
            {
                Transform grandchild = child.GetChild(j);
                grandchild.gameObject.SetActive(false);
            }
        }
    }

    public PrefabPool AddPool(GameObject objectIdentifier, PrefabPool pool)
    {
        poolDictionary.Add(objectIdentifier, pool);
        return pool;
    }

    public PrefabPool GetPool(GameObject objectIdentifier)
    {
        PrefabPool pool;
        if (poolDictionary.TryGetValue(objectIdentifier, out pool))
        {
            return pool;
        }
        else
        {
            pool = new PrefabPool(objectIdentifier, defaultPoolSize, this, expandableByDefault, poolContainer);
            poolDictionary.Add(objectIdentifier, pool);
            return pool;
        }
    }

    public void RemovePool(GameObject objectIdentifier)
    {
        poolDictionary.Remove(objectIdentifier);
    }

    public void ClearPools()
    {
        poolDictionary.Clear();
    }

}

[System.Serializable]
public struct PrefabToPool
{
    public int size;
    public GameObject prefab;
}