using UnityEngine;

public class ShootKnockback : MonoBehaviour
{
    public SVector2 aimDirection;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(float knockbackForce)
    {
        rb.MovePosition((Vector2)transform.position + aimDirection.Value * -knockbackForce);
    }
}
