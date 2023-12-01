using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public SFloat playerHealth;
    public float maxHealth = 100f;

    public RectTransform uiTransform;
    private float startWidth;

    private void Start()
    {
        startWidth = uiTransform.rect.width;
    }

    void Update()
    {
        float playerHealthPercentage = playerHealth.Value / maxHealth;
        uiTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startWidth * playerHealthPercentage);
    }
}
