using System;
using UnityEngine;
using UnityEngine.AI;

public class FollowAgent : MonoBehaviour, IPoolable
{
    [Header("Parameters")]
    public float maxforce = 300f;
    public float maxSpeed = 1.0f;
    public float maxDistance = 10f;
    public float avoidanceRadius = 0.5f;
    [Range(0f, 1f)] public float positionStrength = 1f;

    [Header("References")]
    public PoolManager poolManager;
    public GameObject agentPrefab;

    private PrefabPool agentPool;
    private Rigidbody2D rb;

    private GameObject agent;
    private NavMeshAgent agentNavMesh;
    private AgentTargeter agentTargeter;

    private bool followAgent = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        agentPool = poolManager.GetPool(agentPrefab);
    }

    public void InitialiseAgent()
    {
        agent = agentPool.GetUnusedObject(false);
        agentNavMesh = agent.GetComponent<NavMeshAgent>();
        agentTargeter = agent.GetComponent<AgentTargeter>();
        agentTargeter.SetFollower(this);
        agentNavMesh.speed = maxSpeed;
        agentNavMesh.Warp(transform.position);
        agent.transform.localScale = transform.localScale;
        agentNavMesh.radius = avoidanceRadius;
        agent.SetActive(true);
        followAgent = true;
    }

    private void ClearAgent()
    {
        if (agent == null) return;
        if (agentNavMesh.isOnNavMesh)
            agentNavMesh.ResetPath();
        agent.SetActive(false);
        agent = null;
        agentNavMesh = null;
    }

    public void FixedUpdate()
    {
        if (!followAgent || agent == null) return;
        if (Vector3.Distance(transform.position, agent.transform.position) > maxDistance)
        {
            agentNavMesh.Warp(transform.position);
            if (agentNavMesh.isOnNavMesh)
                agentNavMesh.ResetPath();
        }
        Vector3 deltaPos = agent.transform.position - transform.position;
        float magnitude = deltaPos.magnitude;
        if (magnitude > maxSpeed)
            deltaPos = deltaPos.normalized * maxSpeed;
        rb.AccelerateTo(
            1f / Time.fixedDeltaTime *
            deltaPos *
            Mathf.Pow(positionStrength, 60f * Time.fixedDeltaTime),
            maxforce
            );
    }

    public void PauseFollowing(float duration)
    {
        followAgent = false;
        Invoke("ResumeFollowing", duration);
    }

    public void ResumeFollowing()
    {
        agent.transform.position = transform.position;
        followAgent = true;
    }

    public void Reuse()
    {
        ClearAgent();
        followAgent = false;
    }
}
