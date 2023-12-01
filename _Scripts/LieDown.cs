using UnityEngine;

public class LieDown : MonoBehaviour
{
    public Vector2 layingDownOffset;
    private bool isLayingDown = false;
    private bool layingToTheLeft = false;
    public void SetLayingDown(bool layingDown)
    {
        isLayingDown = layingDown;
        Vector3 position = transform.localPosition;
        if (layingDown)
        {
            Vector3 eulerRotation = transform.rotation.eulerAngles;
            bool layingLeft = Random.Range(0f, 1f) > 0.5;
            layingToTheLeft = layingLeft;
            eulerRotation.z = layingToTheLeft ? 90f : -90f;
            position.x += layingToTheLeft ? layingDownOffset.x : -layingDownOffset.x;
            position.y += layingDownOffset.y;
            transform.rotation = Quaternion.Euler(eulerRotation);
        }
        else
        {
            position.x -= layingToTheLeft ? layingDownOffset.x : -layingDownOffset.x;
            position.y -= layingDownOffset.y;
            transform.rotation = Quaternion.identity;
        }
        transform.localPosition = position;
    }
}
