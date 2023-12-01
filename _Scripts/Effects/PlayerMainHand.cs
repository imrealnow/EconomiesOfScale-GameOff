using UnityEngine;

public class PlayerMainHand : MonoBehaviour
{
    public SVector2 aimDirection;
    public int baseSortingOrder = 2;

    private new SpriteRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        bool isAimingUp = aimDirection.Value.y > 0;
        bool isAimingRight = aimDirection.Value.x > 0;
        int sortingOrder = baseSortingOrder;
        if (isAimingUp) sortingOrder--;
        else if (!isAimingRight) sortingOrder++;
        if (!isAimingUp && !isAimingRight) sortingOrder++;
        renderer.sortingOrder = sortingOrder;
    }
}

