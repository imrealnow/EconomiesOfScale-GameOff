using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "SniperWeapon", menuName = "SO/Weapons/Sniper")]
public class SniperWeapon : Weapon
{
    public SBool showLaser;
    public float warmupTime = 0.5f;
    public UnityEvent onActivated;
    protected InputMaster controls;
    protected InputAction primaryFire;
    protected float warmupTimer = 0f;
    protected bool activated = false;

    public override void Initialise(PoolManager poolManager, SInt ammoVariable)
    {
        base.Initialise(poolManager, ammoVariable);
        controls = new InputMaster();
        primaryFire = controls.Player.PrimaryFire;
        controls.Player.Enable();
        primaryFire.Enable();
    }

    public override bool Fire(Vector3 firePoint, Vector3 aimDirection, GameObject shooter)
    {
        // to play effects when the weapon is activated
        if (!activated)
        {
            activated = true;
            onActivated?.Invoke();
        }
        // if not on cooldown, show laser and start warmup
        if (fireTimer >= fireRate.Value)
        {
            warmupTimer += Time.deltaTime;
            showLaser.Value = true;
            if (primaryFire.IsPressed() && warmupTimer >= warmupTime)
            {
                if (!UseAmmo()) return false; // if ammo cost is more than what's available, don't fire
                fireTimer = 0f;
                warmupTimer = 0f;
                FireProjectile(firePoint, aimDirection, shooter);
                return true;
            }
        }
        else
        {
            showLaser.Value = false;
        }
        return false;
    }

    public override void ResetState()
    {
        showLaser.Value = false;
        activated = false;
    }
}
