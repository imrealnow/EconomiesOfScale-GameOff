using UnityEngine;

public class AimOffset : MonoBehaviour
{
    public SVector2 aimDirection;
    public float aimDistance;
    public Vector2 offset;

    private void Update()
    {
        transform.localPosition = aimDirection.Value.normalized * aimDistance + offset;
    }
}
