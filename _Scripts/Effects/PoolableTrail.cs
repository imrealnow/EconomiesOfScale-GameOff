using UnityEngine;

public class PoolableTrail : MonoBehaviour, IPoolable
{
    private TrailRenderer trailRenderer;

    private void OnEnable()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void Reuse()
    {
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
        }
    }
}
