using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "CursorManager", menuName = "SO/Managers/CursorManager")]
public class CursorManager : SManager
{
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;
    public Texture2D clickCursor;
    public Vector2 hotspot = Vector2.zero;
    private CursorState cursorState = CursorState.Default;

    public override void OnEnabled()
    {
        base.OnEnabled();
        SetDefaultCursor();
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
    }

    public override void Update()
    {
        base.Update();
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.isPressed)
        {
            if (cursorState != CursorState.Click)
            {
                SetClickCursor();
                cursorState = CursorState.Click;
            }
        }
        else if (mouse.leftButton.wasReleasedThisFrame)
        {
            if (cursorState != CursorState.Hover)
            {
                SetHoverCursor();
                cursorState = CursorState.Hover;
            }
        }
        else
        {
            if (cursorState != CursorState.Default)
            {
                SetDefaultCursor();
                cursorState = CursorState.Default;
            }
        }
    }

    public void SetHoverCursor()
    {
        if (hoverCursor)
            Cursor.SetCursor(hoverCursor, hotspot, CursorMode.Auto);
    }

    public void SetClickCursor()
    {
        if (clickCursor)
            Cursor.SetCursor(clickCursor, hotspot, CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        if (defaultCursor)
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}

public enum CursorState
{
    Default,
    Hover,
    Click
}
