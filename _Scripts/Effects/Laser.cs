using UnityEngine;

public class Laser : MonoBehaviour
{
    public Color minColor;
    public Color maxColor;
    public float oscillationSpeed;
    public float oscillationAmplitude;

    private LineRenderer lineRenderer;
    private float oscillationOffset;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        oscillationOffset += oscillationSpeed * Time.deltaTime;
        float oscillation = Mathf.Sin(oscillationOffset) * oscillationAmplitude;
        lineRenderer.startColor = Color.Lerp(minColor, maxColor, oscillation);
        lineRenderer.endColor = Color.Lerp(minColor, maxColor, oscillation);
    }
}
