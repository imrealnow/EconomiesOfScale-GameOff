using UnityEngine;

public class PixelSnapper : MonoBehaviour
{
    public int pixelsPerUnit = 32;
    public bool snapToPixel = true;
    private float pixelSize;

    void Start()
    {
        pixelSize = 1f / pixelsPerUnit;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        if (snapToPixel)
        {
            currentPosition.x = Mathf.Round(currentPosition.x / pixelSize) * pixelSize;
            currentPosition.y = Mathf.Round(currentPosition.y / pixelSize) * pixelSize;
        }
        transform.position = currentPosition;
    }
}
