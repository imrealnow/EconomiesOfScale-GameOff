using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Weapon", menuName = "SO/Weapons/Default")]
public class Weapon : ScriptableObject
{
    public SFloat fireRate;
    public float bulletSpread;
    public float projectileScale;
    public int ammoCost;
    public GameObject projectile;
    public UnityEvent onFire;

    protected SInt ammoVariable;
    protected PoolManager poolManager;
    protected PrefabPool projectilePool;

    protected float fireTimer;
    protected Vector3 lastAimDirection = Vector3.up;

    public virtual void Initialise(PoolManager poolManager, SInt ammoVariable)
    {
        this.poolManager = poolManager;
        this.ammoVariable = ammoVariable;
        projectilePool = poolManager.GetPool(projectile);
    }

    protected Vector2 CalculateProjectileDirection(Vector3 aimDirection)
    {
        float angle = Random.Range(-bulletSpread, bulletSpread);
        return (Quaternion.AngleAxis(angle, Vector3.forward) * aimDirection).normalized;
    }

    protected bool UseAmmo()
    {
        if (ammoVariable.Value < ammoCost) return false;
        ammoVariable.Value -= ammoCost;
        return true;
    }

    public void UpdateTimer(float deltaTime)
    {
        fireTimer += deltaTime;
    }

    public virtual bool Fire(Vector3 firePoint, Vector3 aimDirection, GameObject shooter)
    {
        if (fireTimer >= fireRate.Value)
        {
            if (!UseAmmo()) return false; // if ammo cost is more than what's available, don't fire
            fireTimer = 0f;
            FireProjectile(firePoint, aimDirection, shooter);
            return true;
        }
        return false;
    }

    protected void FireProjectile(Vector3 firePoint, Vector3 aimDirection, GameObject shooter)
    {
        GameObject projectile = projectilePool.GetUnusedObject(false);
        ScalableProjectile scalableProjectile = projectile.GetComponent<ScalableProjectile>();
        Vector2 projectileDirection = CalculateProjectileDirection(aimDirection);
        if (projectileDirection.magnitude == 0)
            projectileDirection = lastAimDirection;
        scalableProjectile.SetScale(projectileScale);
        scalableProjectile.SetProjectileSource(shooter);
        scalableProjectile.SetProjectileDirection(projectileDirection);
        projectile.transform.position = firePoint;
        projectile.SetActive(true);
        lastAimDirection = projectileDirection;
        if (onFire != null)
            onFire.Invoke();
    }

    public virtual void ResetState()
    {
        // nothing to reset
    }
}
