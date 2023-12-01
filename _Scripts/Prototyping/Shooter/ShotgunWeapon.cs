using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ShotgunWeapon", menuName = "SO/Weapons/Shotgun")]
public class ShotgunWeapon : Weapon
{
    public int pelletCount;
    public float spreadAngle;
    public UnityEvent onShoot;
    public override bool Fire(Vector3 firePoint, Vector3 aimDirection, GameObject shooter)
    {
        if (fireTimer >= fireRate.Value)
        {
            if (!UseAmmo()) return false; // if ammo cost is more than what's available, don't fire
            if (onShoot != null) onShoot.Invoke();
            fireTimer = 0f;
            for (int i = 0; i < pelletCount; i++)
            {
                Vector3 spread = Random.insideUnitCircle * spreadAngle;
                Vector3 direction = Quaternion.Euler(spread) * aimDirection;
                FireProjectile(firePoint, direction, shooter);
            }
            return true;
        }
        return false;
    }
}
