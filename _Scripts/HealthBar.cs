using UnityEngine;

public class HealthBar : PoolObject
{
    public float showDuration;
    public Vector2 offset;

    private float endTime;

    private new SpriteRenderer renderer;
    private Material healthBarMaterial;
    private Health healthComponent;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        healthBarMaterial = renderer.material;
        SetHealthPercent(1);
    }

    private void FixedUpdate()
    {
        if (Time.time > endTime)
        {
            gameObject.SetActive(false);
            healthComponent.DetachHealthbar();
        }
    }

    public void AttachHealthBar(Transform parent, Health healthComponent)
    {
        transform.SetParent(parent);
        transform.localPosition = offset;
        this.healthComponent = healthComponent;
    }

    public void SetHealthPercent(float percent)
    {
        healthBarMaterial.SetFloat("Percent", Mathf.Clamp(percent, 0, 1));
        endTime = Time.time + showDuration;
    }
}
