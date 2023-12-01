using UnityEngine;
using UnityEngine.Events;

public class PlayerGunSprite : MonoBehaviour
{
    public SFloat currentScale;
    public Transform mainHand;
    public Transform offHand;
    public Vector2 positionOffset = Vector2.zero;
    public SVector2 aimDirection;
    public int baseSortingOrder = 2;
    public float angleOffset = 0f;
    [Space] public UnityEvent onScaleChange;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        currentScale.variableChanged += UpdateScale;
    }

    private void OnDisable()
    {
        currentScale.variableChanged -= UpdateScale;
    }

    private void UpdateScale()
    {
        if (onScaleChange != null) onScaleChange.Invoke();
        transform.localScale = Vector3.one * (currentScale.Value + 0.5f);
    }

    private void FixedUpdate()
    {
        Vector2 direction = (Vector2)mainHand.position - (Vector2)offHand.position;
        float angle = Mathf.Atan2(aimDirection.Value.y, aimDirection.Value.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + angleOffset, Vector3.forward);
        transform.position = (Vector2)offHand.position + positionOffset;
        bool isAimingUp = aimDirection.Value.y > 0;
        bool isAimingRight = aimDirection.Value.x > 0;
        int sortingOrder = baseSortingOrder + (isAimingUp ? -2 : 1) + (isAimingRight ? 1 : 0);
        spriteRenderer.sortingOrder = sortingOrder;
    }
}
