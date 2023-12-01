using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool isOpen = false;
    public bool isLocked = false;
    public float openSpeed = 1f;
    public Interactable interactableComponent;

    private Animator animator;
    private bool wasOpen = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("DoorOpen", isOpen);
        animator.SetFloat("Speed", openSpeed);
    }

    public void LockDoor(bool closeDoorIfOpen)
    {
        if (interactableComponent != null)
        {
            interactableComponent.SetInteractable(false, false);
            interactableComponent.SetTooltipText("Locked");
        }
        isLocked = true;
        if (closeDoorIfOpen && isOpen)
        {
            isOpen = false;
            wasOpen = true;
            CloseDoor();
        }
    }

    public void UnlockDoor()
    {
        isLocked = false;
        if (interactableComponent != null)
        {
            interactableComponent.SetInteractable(true, false);
            interactableComponent.SetTooltipText("Open Door - E");
        }
        if (wasOpen)
        {
            isOpen = true;
            OpenDoor();
            wasOpen = false;
        }
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        if (isOpen) OpenDoor();
        else CloseDoor();
    }

    public void OpenDoor()
    {
        if (!isLocked)
        {
            isOpen = true;
            interactableComponent.SetInteractable(false);
            animator.SetBool("DoorOpen", isOpen);
        }
    }

    public void CloseDoor()
    {
        isOpen = false;
        interactableComponent.SetInteractable(true, false);
        animator.SetBool("DoorOpen", isOpen);
    }
}
