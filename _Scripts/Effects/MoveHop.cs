using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MoveHop : MonoBehaviour
{
    public AnimationCurve hopCurve;
    public float hopHeight = 0.2f;
    public float hopDuration = 0.1f;
    public float velocityThreshold = 0.1f;

    [Space] public UnityEvent onHop;
    [Space] public UnityEvent onLand;

    private Vector3 lastPosition;
    private float lastDistance = 0;
    private bool isHopping = false;
    private Queue distances = new Queue(5);

    private void Start()
    {
        lastPosition = transform.position;
    }

    private Vector2 GetAverageVelocity()
    {
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

    private void FixedUpdate()
    {
        float distance = Vector2.Distance(lastPosition, transform.position) / Time.fixedDeltaTime;
        distances.Enqueue(transform.position - lastPosition);
        if (distances.Count >= 5) distances.Dequeue();
        if (!isHopping && distance > velocityThreshold && distance >= lastDistance)
        {
            StartCoroutine(Hop());
        }
        lastPosition = transform.position;
        lastDistance = distance;
    }

    private IEnumerator Hop()
    {
        float progress = 0f;
        float endTime = Time.time + hopDuration;
        isHopping = true;
        if (onHop != null) onHop.Invoke();
        while (Time.time < endTime)
        {
            progress += Time.deltaTime;
            Vector3 localPos = transform.localPosition;
            Vector2 averageVelocity = GetAverageVelocity();
            Vector3 eulerAngles = transform.eulerAngles;
            localPos.y = hopCurve.Evaluate(progress / hopDuration) * hopHeight;
            transform.localPosition = localPos;
            yield return null;
        }
        isHopping = false;
        if (onLand != null) onLand.Invoke();
    }
}
