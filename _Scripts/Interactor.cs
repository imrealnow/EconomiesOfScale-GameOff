using UnityEngine;

public class Interactor : MonoBehaviour
{

    private InputMaster playerInput;
    private Interactable closestInteractable;
    private GameObject interactableGameObject;

    private void Awake()
    {
        playerInput = new InputMaster();
        playerInput.Player.Interact.performed += ctx => Interact();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable)
        {
            closestInteractable = interactable;
            interactableGameObject = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Interactable>() != null)
        {
            if (interactableGameObject && other.gameObject == interactableGameObject)
            {
                interactableGameObject = null;
                closestInteractable = null;
            }
        }
    }

    public void Interact()
    {
        if (closestInteractable)
        {
            closestInteractable.Interact();
        }
    }
}
