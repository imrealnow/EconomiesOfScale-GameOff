using UnityEngine;
using UnityEngine.AI;

public class AITest : MonoBehaviour
{
    public Transform target;

    private NavMeshAgent agent;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        agent.SetDestination(target.position);
    }
}
