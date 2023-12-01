using UnityEngine;

public class ScaleFoundryController : MonoBehaviour
{
    public bool isUp = false;
    public RoomController roomController;
    public SEvent openUpgradeScreen;
    public PauseManager pauseManager;
    public Interactable interactable;
    public SEvent enemyKilled;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Set(isUp);
    }

    private void OnEnable()
    {
        enemyKilled.sharedEvent += UpdateInteractableTooltip;
    }

    private void OnDisable()
    {
        enemyKilled.sharedEvent -= UpdateInteractableTooltip;
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        isUp = !isUp;
        animator.SetBool("FoundryUp", isUp);
    }

    public void Set(bool up)
    {
        isUp = up;
        animator.SetBool("FoundryUp", isUp);
    }

    public void Interact()
    {
        if (isUp)
        {
            if (roomController.IsCleared)
            {
                pauseManager.Pause();
                openUpgradeScreen.Fire();
            }
            else
            {
                roomController.ActivateRoom();
                interactable.DisableInteraction();
                UpdateInteractableTooltip();
            }
        }
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable.SetInteractable(interactable, false);
    }

    public void UpdateInteractableTooltip()
    {
        if (!isUp)
        {
            interactable.SetTooltipText("Remaining Enemies: " + roomController.EnemiesRemaining);
        }
        else if (roomController.IsCleared)
        {
            interactable.SetTooltipText("Buy Upgrades - E");
        }
        else
        {

            interactable.SetTooltipText("Begin Cleansing - E");
        }
    }
}
