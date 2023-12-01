using UnityEngine;

public class CoinShimmer : MonoBehaviour
{
    public float timeOffsetRange = 0.5f;
    public float duration = 1f;
    public float scrollSpeed = 1f;

    private new SpriteRenderer renderer;
    private Material instancedMaterial;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        instancedMaterial = renderer.material;
        instancedMaterial.SetFloat("_TimeOffset", Random.Range(0, timeOffsetRange));
        instancedMaterial.SetFloat("_Duration", duration);
        instancedMaterial.SetFloat("_ScrollSpeed", scrollSpeed);
    }
}
