using System;
using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public float outlineThickness = 1f;
    public Color outlineColour = Color.white;

    private SpriteRenderer spriteRenderer;
    private Material material;
    private Boolean showOutline;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    private void FixedUpdate()
    {
        material.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        material.SetColor("_OutlineColour", outlineColour);
        material.SetFloat("_OutlineThickness", showOutline ? outlineThickness : 0);
    }

    public void ShowOutline(bool show)
    {
        showOutline = show;
    }


    [ContextMenu("Toggle Outline")]
    public void ToggleOutline()
    {
        ShowOutline(!showOutline);
    }
}
