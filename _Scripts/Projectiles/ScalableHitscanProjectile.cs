using UnityEngine;

[CreateAssetMenu(fileName = "ScalableHitscanProjectile", menuName = "Projectiles/ScalableHitscanProjectile")]
public class ScalableHitscanProjectile : ScalableProjectile
{
    [Header("Hitscan Parameters")]
    [SerializeField] private float maxDistance = 100f; // Max range of hitscan

    protected override void OnEnable()
    {
        base.OnEnable(); // Call base to retain any initial setup

        CheckForHit();
    }

    protected override void Update()
    {
        // do nothing
    }

    private void CheckForHit()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, maxDistance, layerMask);
        // exclude the projectile source from the raycast
        if (hit.collider != null)
        {
            Vector3 relativeVelocity = CollisionContext.GetRelativeVelocity(gameObject, hit.collider.gameObject);
            ApplyEffects(new CollisionContext(hit.point, hit.collider, GetComponent<Collider2D>(), relativeVelocity, hit.normal));


            DisableProjectile();
        }
        else
        {
            // Handle the case where nothing is hit
            DisableProjectile();
        }
    }

    private void DisableProjectile()
    {
        gameObject.SetActive(false);
    }
}
