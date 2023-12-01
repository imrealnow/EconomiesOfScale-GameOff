using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

[CreateAssetMenu(fileName = "PauseManager", menuName = "SO/Managers/PauseManager", order = 1)]
public class PauseManager : SManager
{
    public RunningSet pauseObjects;
    [Space]
    public UnityEvent OnPaused;
    public UnityEvent OnUnpaused;
    public SEvent openPauseScreen;

    private InputMaster inputActions;

    public bool isPaused = false;
    private List<GameObject> disabledObjects = new List<GameObject>();

    public override void OnEnabled()
    {
        base.OnEnabled();
        isPaused = false;
        inputActions = new InputMaster();
        inputActions.Player.Pause.performed += ctx => TogglePause(ctx);
        inputActions.Enable();
    }

    public override void OnDisabled()
    {
        isPaused = false;
        inputActions.Disable();
        inputActions.Player.Pause.performed -= ctx => TogglePause(ctx);
    }

    public void TogglePause(CallbackContext context)
    {
        SetIsPaused(!isPaused, true);
    }

    public void SetIsPaused(bool paused, bool showPauseScreen = false)
    {
        if (isPaused == paused)
            return;

        if (paused)
            DisablePauseObjects();
        else
            EnablePauseObjects();

        Time.timeScale = paused ? 0f : 1f;

        isPaused = paused;

        if (isPaused && OnPaused != null)
            OnPaused.Invoke();
        if (!isPaused && OnUnpaused != null)
            OnUnpaused.Invoke();
        if (isPaused && showPauseScreen && openPauseScreen != null)
            openPauseScreen.Fire();
    }

    public void Pause()
    {
        SetIsPaused(true);
    }

    public void Unpause()
    {
        SetIsPaused(false);
    }

    private void DisablePauseObjects()
    {
        if (pauseObjects != null)
        {
            disabledObjects.Clear();
            if (pauseObjects.GetSet() == null) return;
            if (pauseObjects.GetSet().Count == 0) return;
            List<GameObject> pauseObjectsCopy = new List<GameObject>(this.pauseObjects.GetSet());
            foreach (var pauseObject in pauseObjectsCopy)
            {
                if (pauseObject == null)
                    continue;
                if (pauseObject.activeInHierarchy)
                {
                    disabledObjects.Add(pauseObject);
                    pauseObject.SetActive(false);

                }
            }
        }
    }

    private void EnablePauseObjects()
    {
        foreach (var pauseObject in disabledObjects)
        {
            if (pauseObject == null)
                continue;
            pauseObject.SetActive(true);
        }
        disabledObjects.Clear();
    }
}
