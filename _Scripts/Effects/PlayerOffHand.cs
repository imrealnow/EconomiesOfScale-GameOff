using UnityEngine;

public class PlayerOffHand : MonoBehaviour
{
    public SVector3 playerPosition;
    public SVector2 aimDirection;
    public Transform mainHand;
    public float mainHandCloseFactor = 0.5f;
    public float distanceFromPlayer = 0.3f;
    public float verticalFactor = 0.5f;
    public float horizontalFactor = 1f;
    public float moveSpeed = 1f;
    public int baseSortingOrder = 2;
    public Vector2 positionOffset = Vector2.zero;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        bool isAimingUp = aimDirection.Value.y > 0;
        bool isAimingRight = aimDirection.Value.x > 0;
        Vector2 position = new Vector2(
                isAimingRight ? -horizontalFactor : horizontalFactor,
                isAimingUp ? verticalFactor : -verticalFactor
            ) * distanceFromPlayer;
        if (mainHand) position = Vector2.Lerp(position, (Vector2)mainHand.position - (Vector2)transform.position, mainHandCloseFactor);
        Vector2 currentPosition = transform.position;
        transform.position = Vector2.MoveTowards(
                    currentPosition,
                    (Vector2)playerPosition.Value + position * distanceFromPlayer + positionOffset,
                    moveSpeed
                );
        int sortingOrder = baseSortingOrder;
        if (isAimingUp) sortingOrder -= 2;
        if (!isAimingUp && isAimingRight) sortingOrder += 2;
        spriteRenderer.sortingOrder = sortingOrder;
    }
}
