using UnityEngine;

public class LightController : MonoBehaviour
{
    public SBool alarmOn;
    public SEvent roomActivated;
    public SEvent roomDeactivated;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        roomActivated.sharedEvent += UpdateAlarm;
        roomDeactivated.sharedEvent += UpdateAlarm;
    }

    private void OnDisable()
    {
        roomActivated.sharedEvent -= UpdateAlarm;
        roomDeactivated.sharedEvent -= UpdateAlarm;
    }

    private void UpdateAlarm()
    {
        animator.SetBool("AlarmOn", alarmOn.Value);
    }
}
