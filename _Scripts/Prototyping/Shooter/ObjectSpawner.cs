using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{

    public GameObject prefabObject;
    public PoolManager poolManager;
    public float spawnRate = 1f;
    public LayerMask spawnLayerMask;
    public Collider2D spawnBounds;

    private PrefabPool objectPool;
    private float spawnTimer = 0f;

    private void Start()
    {
        objectPool = poolManager.GetPool(prefabObject);
    }

    void Update()
    {
        if (spawnTimer > spawnRate)
        {
            SpawnObject();
            spawnTimer = 0f;
        }
        spawnTimer += Time.deltaTime;
    }

    public void SpawnObject()
    {
        GameObject gameObject = objectPool.GetUnusedObject(false);
        Collider2D collider = gameObject.GetComponent<Collider2D>();
        Vector3 spawnPosition = GetRandomSpawnPosition(collider);
        gameObject.transform.position = spawnPosition;
        gameObject.SetActive(true);
    }

    private Vector3 GetRandomSpawnPosition(Collider2D objectCollider = null)
    {
        int attempts = 0;
        Vector3 spawnPosition = Vector3.zero;
        while (attempts < 5)
        {
            Bounds bounds = spawnBounds.bounds;
            spawnPosition = new Vector3(
                                   Random.Range(bounds.min.x, bounds.max.x),
                                   Random.Range(bounds.min.y, bounds.max.y),
                                   0f);
            if (objectCollider != null)
            {
                if (Physics2D.OverlapBox(spawnPosition, objectCollider.bounds.size, 0f, spawnLayerMask) == null)
                {
                    return spawnPosition;
                }
            }
            attempts++;
        }
        return spawnPosition;
    }

}
