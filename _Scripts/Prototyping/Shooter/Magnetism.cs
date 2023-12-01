using UnityEngine;

public class Magnetism : MonoBehaviour
{
    public Vector3Reference targetPosition;
    public float minDistance = 2f;
    public float attractionSpeed = 1.0f;

    private Vector3 lastVelocity = Vector3.zero;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, targetPosition.Value);
        if (distance < minDistance)
        {
            Vector3 targetVelocity = Vector3.Normalize(targetPosition.Value - transform.position);
            targetVelocity *= Mathf.Lerp(attractionSpeed, attractionSpeed / distance, 0.75f) * Time.deltaTime;
            transform.position = transform.position + Vector3.Slerp(targetVelocity, lastVelocity, 0.5f);
            lastVelocity = targetVelocity;
        }
    }
}
