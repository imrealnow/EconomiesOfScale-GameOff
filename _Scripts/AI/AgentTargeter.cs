using UnityEngine;
using UnityEngine.AI;

public class AgentTargeter : MonoBehaviour, IPoolable
{
    public SVector3 targetPosition;
    public float maxDistance = 5f;

    private FollowAgent agentFollower;
    private NavMeshAgent agent;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetFollower(FollowAgent follower)
    {
        agentFollower = follower;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, agentFollower.transform.position) > maxDistance)
        {
            bool successfullyWarped = agent.Warp(agentFollower.transform.position);
            if (agent.isOnNavMesh)
                agent.ResetPath();
        }
        agent.SetDestination(targetPosition.Value);
    }

    public void Reuse()
    {
        agentFollower = null;
    }
}
