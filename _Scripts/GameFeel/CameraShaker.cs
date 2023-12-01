using Cinemachine;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public SEvent shakeTrigger;

    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        shakeTrigger.sharedEvent += Shake;
    }

    private void OnDisable()
    {
        shakeTrigger.sharedEvent -= Shake;
    }

    public void Shake()
    {
        impulseSource.GenerateImpulse();
    }
}
