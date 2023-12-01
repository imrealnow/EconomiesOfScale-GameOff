using UnityEngine;

public class LaserGuide : MonoBehaviour
{
    [Header("Parameters")]
    public LayerMask layerMask;
    public float maxDistance = 100f;
    public float laserWidth = 0.1f;
    public float laserHeight = 0.25f;
    public float angleOffset = 0f;
    public Vector2 positionOffset = Vector2.zero;
    public BoolReference showLaser;

    [Header("References")]
    public Transform firePoint;
    public LineRenderer lineRenderer;
    public SVector2 aimDirection;
    public SVector3 aimOrigin;

    private void Awake()
    {
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.enabled = showLaser.Value;
    }

    private void Update()
    {
        lineRenderer.enabled = showLaser.Value;
        Vector2 laserDirection = aimDirection.Value.normalized;
        Vector2 laserPosition = (Vector2)aimOrigin.Value + positionOffset + laserDirection * 0.4f;
        if (showLaser.Value)
        {
            RaycastHit2D hit = Physics2D.Raycast(laserPosition, laserDirection, maxDistance, layerMask);
            if (hit && !hit.collider.isTrigger)
            {
                Vector3 endPoint = hit.point;
                if (Vector3.Dot(hit.normal, Vector3.down) > 0.9f)
                {
                    // it is a top down 2d game, so we want the laser to be projected onto vertical surfaces by the laser's height
                    // the laser should maintain its direction, it should just be extended vertically to the height of the laser
                    endPoint = hit.point + hit.normal * -laserHeight;
                }
                lineRenderer.SetPosition(0, laserPosition);
                lineRenderer.SetPosition(1, endPoint);
            }
            else
            {
                lineRenderer.SetPosition(0, laserPosition);
                lineRenderer.SetPosition(1, laserPosition + aimDirection.Value * maxDistance);
            }
        }
        else
        {
            lineRenderer.SetPosition(0, laserPosition);
            lineRenderer.SetPosition(1, laserPosition);
        }
    }
}
