using System.Collections;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public float flashDuration = 0.05f;
    public float rotationOffset = 90f;
    public int baseSortingOrder = 2;
    public Vector3 positionOffset = Vector3.zero;
    public SVector2 aimDirection;
    private new SpriteRenderer renderer;
    private Vector3 startLocalPosition;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
        startLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = startLocalPosition + positionOffset;
    }

    public void ShowFlash()
    {
        StartCoroutine(Flash(aimDirection.Value));
    }

    private IEnumerator Flash(Vector3 flashDirection)
    {
        int sortingOrder = baseSortingOrder;
        if (Vector3.Dot(flashDirection, Vector3.up) > 0)
        {
            sortingOrder--;
        }
        else
        {
            sortingOrder++;
        }

        float zRotation = Mathf.Atan2(flashDirection.y, flashDirection.x) * Mathf.Rad2Deg + rotationOffset;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, zRotation));
        renderer.enabled = true;
        renderer.sortingOrder = sortingOrder;
        yield return new WaitForSeconds(flashDuration);
        renderer.enabled = false;
    }
}
