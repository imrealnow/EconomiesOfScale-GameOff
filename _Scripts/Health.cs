using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IPoolable
{
    public float maxHealth = 100;
    public FloatReference currentHealth;
    public bool showHealthbar = true;
    public Transform healthbarParent;
    public GameObject healthBarPrefab;

    public UnityEvent AfterInitialised;
    public UnityEvent OnHeal;
    public UnityEvent OnDamage;
    public UnityEvent OnDeath;

    public PoolManager poolManager;
    private PrefabPool pool;
    private HealthBar healthBar = null;
    private bool isDamagable = true;
    private bool isDead = false;

    public float HealthPercentage => currentHealth.Value / maxHealth;

    void Start()
    {
        currentHealth.Value = maxHealth;
        if (AfterInitialised != null)
            AfterInitialised.Invoke();
        pool = poolManager.GetPool(healthBarPrefab);
    }

    public void ChangeByAmount(float amount)
    {
        if (isDead) return;
        if (amount > 0 && currentHealth.Value != maxHealth && OnHeal != null)
            OnHeal.Invoke();
        if (amount < 0)
        {
            if (!isDamagable) return;
            if (OnDamage != null && currentHealth.Value + amount > 0)
                OnDamage.Invoke();
        }

        currentHealth.Value = Mathf.Clamp(currentHealth.Value + amount, 0, maxHealth);

        if (currentHealth.Value == 0)
        {
            isDead = true;
            OnDeath?.Invoke();
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false);
                DetachHealthbar();
            }
        }
        else if (showHealthbar)
        {
            if (healthBar == null)
            {
                healthBar = pool.GetUnusedObject().GetComponent<HealthBar>();
                if (healthbarParent != null)
                    healthBar.AttachHealthBar(healthbarParent, this);
                else
                    healthBar.AttachHealthBar(transform, this);
            }
            healthBar.SetHealthPercent((float)currentHealth.Value / (float)maxHealth);
            healthBar.gameObject.SetActive(true);
        }
    }

    public void DetachHealthbar()
    {
        healthBar = null;
    }

    public void Reuse()
    {
        currentHealth.Value = maxHealth;
        healthBar = null;
        isDead = false;
    }

    public void SetDamagable(bool isDamagable)
    {
        this.isDamagable = isDamagable;
    }
}
