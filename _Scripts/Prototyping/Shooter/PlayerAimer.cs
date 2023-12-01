using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimer : MonoBehaviour
{
    public Transform firePoint;
    public Vector2Reference aimDirection;
    public Vector2 lastAimDirection = Vector2.up;
    public Vector2 positionOffset;
    public SpriteRenderer playerRenderer;
    public Sprite facingDown;
    public Sprite facingUp;

    private void FixedUpdate()
    {
        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.value) + positionOffset;
        Vector2 direction = mousePosition - (Vector2)transform.position;
        aimDirection.Value = direction;
        playerRenderer.flipX = aimDirection.Value.x < 0;
        if (aimDirection.Value.y < 0)
        {
            playerRenderer.sprite = facingDown;
        }
        else
        {
            playerRenderer.sprite = facingUp;
        }
        if (aimDirection.Value != Vector2.zero)
        {
            lastAimDirection = aimDirection.Value;
        }
        else
        {
            aimDirection.Value = lastAimDirection;
        }
    }
}
