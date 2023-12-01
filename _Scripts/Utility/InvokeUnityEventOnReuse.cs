using UnityEngine;
using UnityEngine.Events;

public class InvokeUnityEventOnReuse : MonoBehaviour, IPoolable
{
    public UnityEvent eventToInvoke;

    public void Reuse()
    {
        if (eventToInvoke != null)
        {
            eventToInvoke.Invoke();
        }
    }
}
