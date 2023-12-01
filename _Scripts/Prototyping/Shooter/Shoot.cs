using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shoot : MonoBehaviour
{
    private static readonly int PRIMARY = 1;
    private static readonly int SECONDARY = 2;

    [Header("Parameters")]
    public List<WeaponSettings> primaryWeapons = new List<WeaponSettings>();
    public List<WeaponSettings> secondaryWeapons = new List<WeaponSettings>();
    public float fireRange = 10f;
    public Vector2 fireOffset = Vector2.zero;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public SInt ammoVariable;
    public int baseAmmoCost;
    public AnimationCurve ammoCostScaleCurve;

    [Header("Fire Effects")]
    public UnityEvent onFire;

    [Header("References")]
    public Transform firePoint;
    public SFloat currentScale;
    public PauseManager pauseManager;

    private PoolManager poolManager;
    private ScaleManager scaleManager;
    private InputMaster inputActions;

    private Vector2 aimDirection = Vector2.up;
    private Boolean initialised = false;
    private Weapon weaponFiredLastFrame = null;

    private async void Start()
    {
        poolManager = await ManagerRegistry.Instance.GetManagerAsync<PoolManager>();
        scaleManager = await ManagerRegistry.Instance.GetManagerAsync<ScaleManager>();
        InitialiseWeapons(primaryWeapons);
        InitialiseWeapons(secondaryWeapons);
        inputActions = new InputMaster();
        inputActions.Enable();
        initialised = true;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void InitialiseWeapons(List<WeaponSettings> weapons)
    {
        foreach (WeaponSettings weapon in weapons)
        {
            weapon.weapon.Initialise(poolManager, ammoVariable);
        }
    }

    private Weapon GetWeapon(int weaponType, Scale scale)
    {
        foreach (var weapon in weaponType == PRIMARY ? primaryWeapons : secondaryWeapons)
        {
            if (weapon.scale == scale)
            {
                return weapon.weapon;
            }
        }
        return null;
    }

    private void UpdateWeaponTimers(List<WeaponSettings> weapons, float deltaTime)
    {
        foreach (var weapon in weapons)
        {
            weapon.weapon.UpdateTimer(deltaTime);
        }
    }

    public void Fire(int weaponType)
    {
        Scale scale = scaleManager.Scale;
        Weapon weapon = GetWeapon(weaponType, scale);
        if (weapon != null)
        {
            if (weaponFiredLastFrame != null && weaponFiredLastFrame != weapon)
                weaponFiredLastFrame.ResetState();
            weaponFiredLastFrame = weapon;
            if (weapon.Fire(firePoint.position + (Vector3)fireOffset, aimDirection, gameObject))
            {
                if (onFire != null) onFire.Invoke();
            }
        }
    }

    public void Update()
    {
        if (!initialised)
            return;
        if (pauseManager.isPaused)
            return;

        aimDirection = (firePoint.position - transform.position).normalized;
        UpdateWeaponTimers(primaryWeapons, Time.deltaTime);
        UpdateWeaponTimers(secondaryWeapons, Time.deltaTime);

        if (inputActions.Player.SecondaryFire.IsPressed())
            Fire(SECONDARY);
        else if (inputActions.Player.PrimaryFire.IsPressed())
            Fire(PRIMARY);
        else if (weaponFiredLastFrame != null)
            weaponFiredLastFrame.ResetState();
    }
}

[Serializable]
public class WeaponSettings
{
    public Scale scale;
    public Weapon weapon;
}
