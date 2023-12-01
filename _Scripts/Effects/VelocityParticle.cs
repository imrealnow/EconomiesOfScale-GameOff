using UnityEngine;

public class VelocityParticle : MonoBehaviour, IPoolable
{
    public float drag = 1f;

    private Vector3 velocity;

    public void ApplyVelocity(Vector3 velocity)
    {
        velocity = velocity.normalized;
    }

    private void FixedUpdate()
    {
        if (velocity.magnitude > 0.05f)
        {
            ApplyDrag();
            transform.position = transform.position + velocity;
        }
        else
        {
            velocity = Vector3.zero;
        }
    }

    private void ApplyDrag()
    {
        float speed = velocity.magnitude;
        float dragForce = drag * speed * speed;
        Vector3 dragDirection = -velocity.normalized;

        // apply the drag force to the velocity
        velocity += dragDirection * dragForce * Time.deltaTime;
    }

    public void Reuse()
    {
        velocity = Vector3.zero;
    }
}
