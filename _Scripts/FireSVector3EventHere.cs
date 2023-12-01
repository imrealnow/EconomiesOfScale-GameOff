using UnityEngine;

public class FireSVector3EventHere : MonoBehaviour
{
    public SVector3Event eventToFire;

    public void FireEvent()
    {
        if (eventToFire != null)
        {
            eventToFire.Fire(transform.position);
        }
    }
}
