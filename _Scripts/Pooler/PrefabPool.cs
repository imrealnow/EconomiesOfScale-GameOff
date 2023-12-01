using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabPool
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private int poolAmount;
    [SerializeField]
    private bool canExpand;

    private GameObject _parentObject;
    private PoolManager _poolManager;

    private List<GameObject> poolList = new List<GameObject>();
    private List<IPoolable> resettableComponents = new List<IPoolable>();

    public List<GameObject> PoolList
    {
        get { return poolList; }
    }

    public GameObject ParentObject => _parentObject;

    public PrefabPool(GameObject prefabToPool, int poolSize, PoolManager poolManager, bool expandable, GameObject parentObject)
    {
        _poolManager = poolManager;
        _parentObject = new GameObject(prefabToPool.name + "Pool");
        _parentObject.transform.SetParent(parentObject.transform);

        prefab = prefabToPool;
        poolAmount = poolSize;
        canExpand = expandable;
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Object.Instantiate(prefabToPool);
            obj.SetActive(false);
            obj.transform.SetParent(_parentObject.transform);
            PoolObject objectNamer = obj.GetComponent<PoolObject>();
            if (objectNamer == null)
                objectNamer = obj.AddComponent<PoolObject>();
            objectNamer.prefabPool = this;
            objectNamer.OriginalName = prefab.name;
            poolList.Add(obj);
        }
    }

    public GameObject GetUnusedObject(bool setObjectActive = true)
    {
        foreach (var obj in poolList) // look for inactive objects
        {
            if (obj == null) continue;
            if (!obj.activeInHierarchy)
            {
                ResetPoolObject(obj);
                obj.SetActive(setObjectActive);
                return obj; // give the gameobject to the method that called it
            }
        }
        if (canExpand)
        {
            poolAmount++;
            GameObject obj = Object.Instantiate(prefab);
            obj.transform.SetParent(_parentObject.transform);
            obj.SetActive(false);
            resettableComponents = obj.GetComponents<IPoolable>().ToList();
            if (resettableComponents.Count > 0)
            {
                foreach (IPoolable resetableComponent in resettableComponents)
                {
                    resetableComponent.Reuse();
                }
            }
            poolList.Add(obj);
            PoolObject objectNamer = obj.GetComponent<PoolObject>();
            if (objectNamer == null)
                objectNamer = obj.AddComponent<PoolObject>();
            objectNamer.prefabPool = this;
            objectNamer.OriginalName = prefab.name;
            obj.SetActive(setObjectActive);
            return obj;
        }
        return null;
    }

    public void ShrinkPool(int byAmount = -1)
    {
        foreach (GameObject obj in poolList)
        {
            if (!obj.activeInHierarchy)
            {
                if (byAmount < 0) // -1 just means remove all unused objects
                {
                    poolList.Remove(obj);
                }
                else if (byAmount > 0)
                {
                    byAmount--;
                    poolList.Remove(obj);
                }
                else
                    return;
            }
        }
    }

    public void ResetPool()
    {
        foreach (GameObject repooledObject in poolList)
        {
            repooledObject.SetActive(false);
        }
    }

    public void ResetPoolObject(GameObject obj)
    {
        obj.transform.SetParent(_parentObject.transform);
        resettableComponents = obj.GetComponents<IPoolable>().ToList();
        resettableComponents.AddRange(obj.GetComponentsInChildren<IPoolable>());
        if (resettableComponents.Count > 0)
        {
            foreach (IPoolable resetableComponent in resettableComponents)
            {
                resetableComponent.Reuse(); // reset those components
            }
        }
    }
}

