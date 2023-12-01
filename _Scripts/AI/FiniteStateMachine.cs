using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FiniteStateMachine : MonoBehaviour
{
    public FiniteStateObject initialState;
    public NavMeshAgent agent;
    public GameObject aiCharacterObject;
    public float detectionRadius;
    public float lineOfSightDistance = 10f;
    public float rayWidth = 0.5f;
    public LayerMask rayCastMask;

    public event Action<FiniteStateObject> onStateChanged;

    private IFiniteState currentState;
    private Coroutine coroutine;
    private Cooldown stateFrequency;
    public Vector2 targetPosition { get; set; }
    private bool freezeExecution;

    private void Awake()
    {
        if (initialState != null)
            ChangeToState(initialState);
    }

    private void FixedUpdate()
    {
        if (freezeExecution) return;
        if (stateFrequency.TryUseCooldown())
        {
            IFiniteState next = null;
            if (currentState != null)
                next = currentState.Update();
            // state wants to change
            if (next != null && next != currentState)
            {
                ChangeToState(next);
            }
        }
    }

    public void SetPathfindingAgentSpeed(float speed)
    {
        if (agent != null)
        {
            agent.speed = speed;
        }
    }

    public float GetPathfindingAgentSpeed(float speed)
    {
        if (agent != null)
            return agent.speed;
        return 0;
    }

    public void SetExecuting(bool executing)
    {
        freezeExecution = !executing;
    }

    public void ChangeToState(IFiniteState state, params object[] args)
    {
        // don't change state if it's already the current state
        if (currentState == state) return;
        // clean up current state
        if (currentState != null)
            currentState.OnLeave();
        currentState = state.Initialise(this);
        currentState.OnEnter();
        // apply additional parameters if they exist
        if (args.Length > 0)
            currentState.IngestArguments(args);
        stateFrequency = new Cooldown(currentState.GetUpdateFrequency());
        if (state is FiniteStateObject)
            onStateChanged?.Invoke(state as FiniteStateObject);
    }

    // helper method to allow for finite states to run coroutines
    public Coroutine RunCoroutine(IEnumerator routine)
    {
        coroutine = StartCoroutine(routine);
        return coroutine;
    }

    public void CancelCoroutine(Coroutine routine)
    {
        if (coroutine != null)
            StopCoroutine(routine);
    }

    /// <summary>
    /// Searches all colliders within the detection radius that match the searchQuery
    /// </summary>
    /// <param name="searchQuery">Predicate to run on any GameObjects found</param>
    /// <returns>Returns the nearest GameObject that matches the searchQuery if one exists, else returns null</returns>
    public GameObject SearchForNearest(Predicate<GameObject> searchQuery)
    {
        Collider2D[] detectionHits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        float minDistance = float.PositiveInfinity;
        GameObject closest = null;
        foreach (Collider2D hit in detectionHits)
        {
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (searchQuery.Invoke(hit.gameObject) && distance < minDistance)
            {
                minDistance = distance;
                closest = hit.gameObject;
            }
        }
        return closest;
    }

    public void MoveTo(Vector2 position)
    {
        if (agent != null)
        {
            agent.SetDestination(position);
        }
    }

    public bool CanSeeObject(GameObject obj)
    {
        Vector2 direction = obj.transform.position - transform.position;

        RaycastHit2D rayHit = Physics2D.CircleCast((Vector2)transform.position + direction * rayWidth, rayWidth, direction, lineOfSightDistance, rayCastMask);
        if (rayHit.collider != null && rayHit.collider.gameObject == obj)
        {
            return true;
        }
        return false;
    }

    public IFiniteState GetCurrentState()
    {
        return currentState;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}


