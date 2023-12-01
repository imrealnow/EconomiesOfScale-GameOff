using UnityEngine;

public class ProgressBar : PoolObject
{
    [Range(0.01f, 100f)]
    public float duration;

    private SpriteRenderer spriteRenderer;
    private Material material;
    private float endTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    protected override void OnEnable()
    {
        endTime = Time.time + duration;
    }

    private void FixedUpdate()
    {
        if (Time.time > endTime)
        {
            gameObject.SetActive(false);
        }
        else
        {
            material.SetFloat("Percent", Mathf.Clamp((endTime - Time.time) / duration, 0, 1));
        }
    }


}
