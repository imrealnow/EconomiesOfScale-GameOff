using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LinearProjectile : PooledProjectile, IPoolable
{
    public Vector3 direction;
    public float speed;
    public float minCollisionDistance;
    public LayerMask layerMask;

    private new Collider2D collider2D;
    private new ParticleSystem particleSystem;
    private int enabledFrameCount;

    private bool projectileHit = false;

    private void Start()
    {
        collider2D = GetComponent<Collider2D>();
        particleSystem = GetComponent<ParticleSystem>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (particleSystem != null)
            particleSystem.Play();
        enabledFrameCount = Time.frameCount;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        if (Time.frameCount >= enabledFrameCount + 3)
        {
            RaycastHit2D collision = CheckCollision();
            if (collision.collider != null)
            {
                if (!projectileHit)
                {
                    Vector3 relativeVelocity = GetRelativeProjectileVelocity(collision, direction * speed);
                    ApplyEffects(new CollisionContext(collision.point, collision.collider, collider2D, relativeVelocity, collision.normal));
                    projectileHit = true;
                }
                DestroyProjectile();
            }
        }
    }



    private void DestroyProjectile()
    {
        if (particleSystem != null)
        {
            direction = Vector2.zero;
            Invoke("DisableProjectile", particleSystem.main.duration);
        }
        else
            DisableProjectile();
    }

    protected override RaycastHit2D CheckCollision()
    {
        RaycastHit2D[] results = new RaycastHit2D[1];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(layerMask);
        int hitcount = collider2D.Cast(direction, filter, results, speed * Time.deltaTime);
        if (hitcount > 0)
        {
            if (results[0].collider.gameObject != projectileSource)
                return results[0];
        }
        return new RaycastHit2D();
    }

    private void DisableProjectile()
    {
        projectileHit = true;
        gameObject.SetActive(false);
    }

    public void Reuse()
    {
        projectileHit = false;
        if (particleSystem != null)
            particleSystem.Clear();
        CancelInvoke();
        direction = Vector2.zero;
    }
}
