using UnityEngine;

public static class Rigidbody2DExtensions
{
    public static void AccelerateTo(this Rigidbody2D rb, Vector3 targetVelocity, float maxAccel)
    {
        Vector3 deltaV = targetVelocity - (Vector3)rb.velocity;
        Vector3 accel = deltaV / Time.deltaTime;

        if (accel.sqrMagnitude > maxAccel * maxAccel)
            accel = accel.normalized * maxAccel;

        rb.AddForce(accel / rb.mass, ForceMode2D.Force);
    }
}
