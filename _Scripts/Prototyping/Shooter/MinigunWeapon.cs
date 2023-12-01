using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MinigunWeapon", menuName = "SO/Weapons/Minigun")]
public class MinigunWeapon : Weapon
{
    [Header("Parameters")]
    public float warmupTime = 0.5f;
    public float cooldownTime = 0.5f;
    public float maxFireDuration = 5f;
    public float maxSpinSpeed = 20f;
    public float minSpinSpeed = 5f;
    public float spinUpTime = 1f;
    public float playerSpeedMultiplier = 0.75f;
    public AnimationCurve spinUpCurve;

    [Header("Events")]
    [Space] public UnityEvent onHold;
    [Space] public UnityEvent onCooldown;

    [Header("References")]
    public SFloat playerSpeed;

    private float cooldownTimer = 0f;
    private float fireDuration = 0f;
    private float basePlayerSpeed = 0f;

    public override void Initialise(PoolManager poolManager, SInt ammoVariable)
    {
        base.Initialise(poolManager, ammoVariable);
        basePlayerSpeed = playerSpeed.Value;
        cooldownTimer = cooldownTime;
    }

    public override bool Fire(Vector3 firePoint, Vector3 aimDirection, GameObject shooter)
    {
        if (onHold != null) onHold.Invoke();
        if (cooldownTimer < cooldownTime)
        {
            cooldownTimer += Time.deltaTime;
            return false;
        }
        else if (fireDuration < maxFireDuration)
        {
            fireDuration += Time.deltaTime;
            float spinSpeed = Mathf.Lerp(minSpinSpeed, maxSpinSpeed, spinUpCurve.Evaluate(fireDuration / spinUpTime));
            float fireRate = 1f / spinSpeed;
            if (fireTimer >= fireRate)
            {
                if (!UseAmmo()) return false; // if ammo cost is more than what's available, don't fire
                playerSpeed.Value = basePlayerSpeed * playerSpeedMultiplier;
                fireTimer = 0f;
                FireProjectile(firePoint, aimDirection, shooter);
                return true;
            }
            return false;
        }
        else
        {
            // begin cooldown
            if (onCooldown != null) onCooldown.Invoke();
            cooldownTimer = 0f;
            fireDuration = 0f;
            playerSpeed.Value = basePlayerSpeed;
            return false;
        }
    }

    public override void ResetState()
    {
        Debug.Log("Resetting state");
        cooldownTimer = cooldownTime;
        fireDuration = 0f;
    }
}
