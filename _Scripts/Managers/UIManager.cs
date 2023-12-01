using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIManager", menuName = "SO/Managers/UIManager")]
public class UIManager : SManager
{
    public GameObject uiCanvasPrefab;
    public GameObject defaultScreen;
    public List<ScreenRegister> registeredScreens = new List<ScreenRegister>();
    public SEvent closeCurrentScreen;
    private Dictionary<ScreenRegister, GameObject> instantiatedScreens = new Dictionary<ScreenRegister, GameObject>();
    private GameObject screenParent;

    private Stack<ScreenRegister> screenStack = new Stack<ScreenRegister>();

    public override void OnEnabled()
    {
        base.OnEnabled();
        screenParent = Instantiate(uiCanvasPrefab);
        screenParent.transform.SetParent(handler.transform);
        closeCurrentScreen.sharedEvent += () => CloseCurrentScreen();
        RegisterScreens();
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        closeCurrentScreen.sharedEvent -= () => CloseCurrentScreen();
        DeregisterScreens();
    }

    private void RegisterScreens()
    {
        foreach (var register in registeredScreens)
        {
            GameObject panelObject = Instantiate(register.panelPrefab, screenParent.transform);
            panelObject.SetActive(false);
            instantiatedScreens.Add(register, panelObject);

            if (register.openTrigger != null)
                register.openTrigger.sharedEvent += () => ChangeScreen(register);

            if (register.closeTrigger != null)
                register.closeTrigger.sharedEvent += () => CloseCurrentScreen();
        }
    }

    private void DeregisterScreens()
    {
        foreach (var register in registeredScreens)
        {
            if (register.openTrigger != null)
                register.openTrigger.sharedEvent -= () => ChangeScreen(register);

            if (register.closeTrigger != null)
                register.closeTrigger.sharedEvent -= () => CloseCurrentScreen();
        }
    }

    public void ChangeScreen(ScreenRegister screen)
    {
        if (screenStack.Count > 0 && screenStack.Peek() == screen)
            return;

        if (screenStack.Count > 0)
        {
            var currentScreen = screenStack.Peek();
            instantiatedScreens[currentScreen].SetActive(false);
        }

        screenStack.Push(screen);
        instantiatedScreens[screen].SetActive(true);
    }

    public void CloseCurrentScreen()
    {
        if (screenStack.Count == 0)
            return;

        var currentScreen = screenStack.Pop();
        if (currentScreen.panelPrefab == defaultScreen)
        {
            screenStack.Push(currentScreen);
            return;
        }
        instantiatedScreens[currentScreen].SetActive(false);

        if (screenStack.Count > 0)
        {
            var previousScreen = screenStack.Peek();
            instantiatedScreens[previousScreen].SetActive(true);
        }
    }
}

[Serializable]
public class ScreenRegister
{
    public GameObject panelPrefab;
    public SEvent openTrigger;
    public SEvent closeTrigger;
}
