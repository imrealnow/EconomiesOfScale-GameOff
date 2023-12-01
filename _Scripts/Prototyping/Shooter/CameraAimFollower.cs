using UnityEngine;
using UnityEngine.InputSystem;

public class CameraAimFollower : MonoBehaviour
{
    // find direction and distance of mouse to screen center
    // then reposition this object to be at the player position + direction * distance
    // if distance is greater than maxDistance, clamp it to maxDistance

    public Transform playerTransform;
    public float minDistance = 0;
    public float maxDistance = 1f;
    public float smoothFactor = 0.5f;
    public int pixelsPerUnit = 32;
    public Vector2 offset = Vector2.zero;

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        Vector3 currentPosition = transform.localPosition;
        Vector3 playerPosition = playerTransform.position + (Vector3)offset;
        Vector3 targetPosition = (mouseWorldPos - playerPosition).normalized
            * Mathf.Clamp((mouseWorldPos - playerPosition).magnitude, minDistance, maxDistance);
        float distance = Vector3.Distance(currentPosition, targetPosition);
        float sigmoidDistance = 1f / (1f + Mathf.Exp(-distance));
        Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPosition, sigmoidDistance * smoothFactor * Time.deltaTime);
        float pixelSize = 1f / pixelsPerUnit;
        newPosition.x = Mathf.Round(newPosition.x / pixelSize) * pixelSize;
        newPosition.y = Mathf.Round(newPosition.y / pixelSize) * pixelSize;
        transform.localPosition = newPosition;
    }
}
