using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract = new UnityEvent();
    public string tooltipText = "Press E to interact";
    public GameObject tooltipObject;
    public OutlineEffect effect;
    public SoundManager soundManager;
    public AudioClip interactionSuccessSound;
    public AudioClip interactionFailSound;

    private TextMeshPro tooltipTextComponent;
    private string interactorTag = "Interactor";
    private bool isInteractable = true;
    private bool showText = true;

    private void Awake()
    {
        if (tooltipObject)
        {
            tooltipTextComponent = tooltipObject.GetComponentInChildren<TextMeshPro>();
            tooltipTextComponent.text = tooltipText;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(interactorTag) && showText)
        {
            if (tooltipObject)
            {
                tooltipObject.SetActive(true);
            }
            effect?.ShowOutline(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals(interactorTag))
        {
            if (tooltipObject)
            {
                tooltipObject.SetActive(false);
            }
            effect?.ShowOutline(false);
        }
    }

    public void SetInteractable(bool enabled, bool hideText = true)
    {
        isInteractable = enabled;
        showText = !hideText;
        if (isInteractable)
        {
            if (tooltipObject && hideText)
            {
                tooltipObject.SetActive(false);
            }
            effect?.ShowOutline(false);
        }
    }

    public void DisableInteraction()
    {
        isInteractable = false;
    }

    public void SetTooltipText(string text)
    {
        tooltipText = text;
        tooltipTextComponent.text = tooltipText;
    }

    public void Interact()
    {
        if (isInteractable)
        {
            onInteract.Invoke();
            if (interactionSuccessSound)
            {
                soundManager.PlaySFX(interactionSuccessSound);
            }
        }
        else
        {
            if (interactionFailSound)
            {
                soundManager.PlaySFX(interactionFailSound);
            }
        }
    }
}
