using UnityEngine;

[CreateAssetMenu(fileName = "ScalableProjectile", menuName = "Projectiles/ScalableProjectile")]
public class ScalableProjectile : PooledProjectile, IPoolable
{
    [Header("Base Parameters")]
    [SerializeField] protected Vector3 direction;
    [SerializeField] protected SFloat baseSpeed;
    [SerializeField] protected SFloat baseDamage;
    [SerializeField] protected LayerMask layerMask;

    [Header("Scaling Parameters")]
    [SerializeField] private AnimationCurve speedScaleCurve;
    [SerializeField] private AnimationCurve damageScaleCurve;

    protected new Collider2D collider2D;
    protected new ParticleSystem particleSystem;
    protected ScaleManager scaleManager;
    protected int enabledFrameCount;

    private bool projectileHit = false;
    private float damage;
    private float speed;

    private void Start()
    {
        collider2D = GetComponent<Collider2D>();
        particleSystem = GetComponent<ParticleSystem>();
        damage = baseDamage.Value;
    }

    public void SetScale(float scale)
    {
        speed = baseSpeed.Value * speedScaleCurve.Evaluate(scaleManager.NormalisedScale);
        damage = baseDamage.Value * damageScaleCurve.Evaluate(scaleManager.NormalisedScale);
        transform.localScale = Vector3.one * scale;
    }

    public void SetProjectileDirection(Vector2 aimDirection)
    {
        direction = aimDirection;
    }

    protected override void ApplyEffects(CollisionContext context)
    {
        if (!ValidateCollision(context)) return;
        if (collisionProvider.CollisionsActive)
        {
            foreach (CollisionEffect effect in collisionProvider.GetEffects())
            {

                effect.ApplyEffect(context, damage);
            }
        }
    }

    protected override async void OnEnable()
    {
        base.OnEnable();
        if (particleSystem != null)
            particleSystem.Play();
        if (scaleManager == null)
            scaleManager = await ManagerRegistry.Instance.GetManagerAsync<ScaleManager>();
        enabledFrameCount = Time.frameCount;
    }

    protected virtual void Update()
    {
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        transform.position = newPosition;
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

    protected void DestroyProjectile()
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
