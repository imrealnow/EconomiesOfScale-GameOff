using UnityEngine;
using UnityEngine.Events;

public class InvokeUnityEventAfterDelay : MonoBehaviour
{
    public UnityEvent eventToInvoke;
    public float delay;

    private void OnEnable()
    {
        Invoke("InvokeUnityEvent", delay);
    }

    private void InvokeUnityEvent()
    {
        if (eventToInvoke != null)
        {
            eventToInvoke.Invoke();
        }
    }
}
