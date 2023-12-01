using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScaleManager", menuName = "SO/Managers/ScaleManager")]
public class ScaleManager : SManager
{
    public Scale scale;
    public SFloat currentScaleFloat;
    public SEvent resetEvent;
    public float changeCooldown = 0.1f;
    public PauseManager pauseManager;
    private InputMaster inputActions;

    private Dictionary<Scale, bool> scalesUnlocked = new Dictionary<Scale, bool>()
    {
        { Scale.Small, true },
        { Scale.Large, false },
        { Scale.Huge, false }
    };

    public int CurrentScale { get => (int)scale; }
    public Scale Scale { get => (Scale)scale; }
    private Cooldown scaleChangeCooldown;

    public float NormalisedScale
    {
        get
        {
            float minScale = 1;
            float maxScale = 3;
            float currentScale = (float)scale;
            float normalizedScale = (currentScale - minScale) / (maxScale - minScale);
            return normalizedScale;
        }
    }

    public override void OnEnabled()
    {
        base.OnEnabled();
        inputActions = new InputMaster();
        inputActions.Player.Scale.performed += ctx => ChangeScale(ctx.ReadValue<float>());
        inputActions.Enable();
        resetEvent.sharedEvent += Reset;
        scale = Scale.Small;
        scaleChangeCooldown = new Cooldown(changeCooldown);
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        resetEvent.sharedEvent -= Reset;
        inputActions.Player.Scale.performed -= ctx => ChangeScale(ctx.ReadValue<float>());
        inputActions.Disable();
    }

    public void Reset()
    {
        scale = Scale.Small;
        scalesUnlocked = new Dictionary<Scale, bool>()
        {
            { Scale.Small, true },
            { Scale.Large, false },
            { Scale.Huge, false }
        };
    }

    private void ChangeScale(float scale)
    {
        if (pauseManager.isPaused)
            return;
        if (scaleChangeCooldown.TryUseCooldown())
        {
            int[] unlockedScales = scalesUnlocked.Keys.Where(key => scalesUnlocked[key]).Select(key => (int)key).ToArray();
            int direction = Math.Sign(scale);
            int currentScale = (int)this.scale;
            int maxScale = unlockedScales.Max();
            int minScale = unlockedScales.Min();
            int nextScale = currentScale + direction;
            if (nextScale > maxScale) nextScale = minScale;
            if (nextScale < minScale) nextScale = maxScale;

            this.scale = (Scale)nextScale;
            if (currentScaleFloat != null) currentScaleFloat.Value = (float)this.scale * 0.5f;
        }
    }

    public void UnlockScale(Scale scale)
    {
        scalesUnlocked[scale] = true;
    }

    public void UnlockLargeScale()
    {
        UnlockScale(Scale.Large);
    }

    public void UnlockHugeScale()
    {
        UnlockScale(Scale.Huge);
    }

    public bool IsScaleUnlocked(Scale scale)
    {
        return scalesUnlocked[scale];
    }
}

public enum Scale
{
    Small = 1, Large = 2, Huge = 3
}
