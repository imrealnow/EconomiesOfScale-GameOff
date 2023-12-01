using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemies")]
    public List<EnemySettings> enemySettings = new List<EnemySettings>();

    [Header("Parameters")]
    public float initialSpawnDelay = 0f;
    public float spawnFrequency = 0.5f;
    public float minDistanceFromPlayer = 10f;
    public float maxDistanceFromPlayer = 10f;
    public int maxEnemies = 30;

    [Header("References")]
    public SVector3 playerPosition;
    public PoolManager poolManager;
    public RunningSet enemySet;

    [Header("Events")]
    public SIntegerEvent spawnTriggerEvent;

    private Dictionary<GameObject, PrefabPool> enemyPools = new Dictionary<GameObject, PrefabPool>();

    private float spawnBudget;
    public float SpawnBudget { get => spawnBudget; set => spawnBudget = value; }

    private float spawnTimer = 0f;

    private void Start()
    {
        InitialiseEnemies();
    }

    private void OnEnable()
    {
        spawnTriggerEvent.sharedEvent += SpawnEnemies;
    }

    private void OnDisable()
    {
        spawnTriggerEvent.sharedEvent -= SpawnEnemies;
    }

    private void InitialiseEnemies()
    {
        enemyPools.Clear();
        foreach (var enemy in enemySettings)
        {
            PrefabPool enemyPool = poolManager.GetPool(enemy.enemyPrefab);
            enemyPools.Add(enemy.enemyPrefab, enemyPool);
        }

    }

    private EnemySettings SelectEnemyWithinBudget()
    {
        List<EnemySettings> affordableEnemies = enemySettings.Where(enemy => enemy.spawnCost <= spawnBudget).ToList();
        if (affordableEnemies.Count == 0)
        {
            return null;
        }
        int randomIndex = UnityEngine.Random.Range(0, affordableEnemies.Count);
        return affordableEnemies[randomIndex];
    }

    private Vector3 RandomNavmeshLocation(float minDistance, float maxDistance, Bounds roomBounds, int maxSearchAttempts = 10)
    {
        Vector2 minPosition = roomBounds.min;
        Vector2 maxPosition = roomBounds.max;
        Quaternion reattemptRotation = Quaternion.Euler(new Vector3(0, 0, (360 / (maxSearchAttempts + 1)) - 180f));
        minDistance = Mathf.Min(minDistance, maxDistance);
        Vector3 finalPosition = playerPosition.Value;
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        while (maxSearchAttempts > 0)
        {
            float randomDistance = Random.Range(minDistance, maxDistance);
            Vector3 targetPosition = playerPosition.Value + randomDirection * randomDistance;
            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition, out hit, maxDistance, NavMesh.AllAreas))
            {
                finalPosition = hit.position;
                if (Vector3.Distance(finalPosition, playerPosition.Value) >= minDistance)
                {
                    break;
                }
            }
            maxSearchAttempts--;
            randomDirection = reattemptRotation * randomDirection;
        }
        return finalPosition;
    }

    private bool SpawnEnemy(EnemySettings selectedEnemy)
    {
        RoomController activeRoomController = RoomController.ActiveRoom;
        if (!activeRoomController)
        {
            Debug.LogError("Cannot spawn enemy: no active room");
            return false;
        }
        PrefabPool enemyPool = enemyPools[selectedEnemy.enemyPrefab];
        GameObject enemy = enemyPool.GetUnusedObject(false);
        FollowAgent enemyFollower = enemy.GetComponent<FollowAgent>();
        enemy.transform.position = RandomNavmeshLocation(
            minDistanceFromPlayer,
            maxDistanceFromPlayer,
            activeRoomController.RoomBounds
        );
        enemyFollower.InitialiseAgent();
        enemy.SetActive(true);
        spawnBudget -= selectedEnemy.spawnCost;
        spawnTimer = 0f;
        return true;
    }

    public void SpawnEnemies(int enemyCount)
    {
        spawnBudget += 15;
        StartCoroutine(SpawnLoop(enemyCount));
    }

    private IEnumerator SpawnLoop(int enemyCount)
    {
        int enemiesSpawned = 0;
        int maxCount = enemyCount + 2 - (enemyCount / 5);
        float spawnDelay = 10f / maxCount + spawnFrequency;
        yield return new WaitForSeconds(initialSpawnDelay);
        while (enemiesSpawned < enemyCount)
        {
            if (enemySet.Count >= maxCount)
            {
                yield return new WaitForSeconds(spawnDelay);
                continue;
            }
            EnemySettings selectedEnemy = SelectEnemyWithinBudget();
            if (selectedEnemy != null)
            {
                if (!SpawnEnemy(selectedEnemy)) yield break;
                enemiesSpawned++;
            }
            else
            {
                spawnBudget += 1f;
            }
            yield return new WaitForSeconds(spawnDelay);
        }
        yield return null;
    }
}

[System.Serializable]
public class EnemySettings
{
    public GameObject enemyPrefab;
    public int spawnCost;
}
