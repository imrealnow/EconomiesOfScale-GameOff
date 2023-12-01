using System.Collections;
using UnityEngine;

public class VelocityInferencer : MonoBehaviour
{
    public int maxSamples = 5;
    private Queue distances = new Queue(5);
    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
        distances.Enqueue(transform.position);
    }

    public Vector2 GetAverageVelocity()
    {
        if (distances.Count == 0)
        {
            return Vector2.zero;
        }
        Vector2 velocity = Vector2.zero;
        foreach (var vector in distances)
        {
            if (vector is Vector2)
            {
                Vector2 vector2 = (Vector2)vector;
                velocity += vector2;
            }
        }
        return velocity / distances.Count;
    }

    public void AddSample(Vector2 sample)
    {
        distances.Enqueue(sample);
        if (distances.Count >= maxSamples) distances.Dequeue();
    }

    public void ClearSamples()
    {
        distances.Clear();
    }

    private void FixedUpdate()
    {
        AddSample(transform.position - lastPosition);
        lastPosition = transform.position;
    }
}
