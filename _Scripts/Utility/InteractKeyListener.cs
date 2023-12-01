using UnityEngine;
using UnityEngine.Events;

public class InteractKeyListener : MonoBehaviour
{
    public UnityEvent onInteract = new UnityEvent();
    private InputMaster inputActions;

    private void Awake()
    {
        inputActions = new InputMaster();
        inputActions.Player.Interact.performed += ctx => onInteract.Invoke();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
