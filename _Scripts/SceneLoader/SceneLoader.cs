using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Parameters")]
    public float sceneUnloadDelay = 1f;
    public bool loadOnAwake = true;
    public int firstSceneIndex;


    [Header("Events")]
    public SEvent sceneLoaded;
    public SEvent sceneUnloaded;

    [Space]
    public List<SceneLoadTrigger> sceneLoadTriggers = new List<SceneLoadTrigger>();

    private int currentSceneIndex = 0;
    private int nextScene;

    private AsyncOperation asyncProgress;


    public float LoadProgress => asyncProgress?.progress ?? 0f;

    private void Awake()
    {
        foreach (SceneLoadTrigger trigger in sceneLoadTriggers)
        {
            if (trigger.triggerEvent != null && SceneManager.GetSceneByBuildIndex(trigger.sceneIndex) != null)
                trigger.triggerEvent.sharedEvent += () => Load(trigger.sceneIndex);
        }

        if (loadOnAwake)
            Load(firstSceneIndex);
    }

    private void OnDestroy()
    {
        foreach (SceneLoadTrigger trigger in sceneLoadTriggers)
        {
            if (trigger.triggerEvent != null)
                trigger.triggerEvent.sharedEvent -= () => Load(trigger.sceneIndex);
        }
    }

    public void Load(int sceneIndex)
    {
        nextScene = sceneIndex;
        Scene currentScene = SceneManager.GetSceneByBuildIndex(currentSceneIndex);

        if (currentScene.isLoaded)
        {
            sceneUnloaded?.Fire();
            if (sceneUnloadDelay > 0)
            {
                StartCoroutine(DelayedLoad());
            }
            else
            {
                SceneManager.sceneUnloaded += LoadScene;
                SceneManager.UnloadSceneAsync(currentSceneIndex);
            }
        }
        else
        {
            LoadScene(currentScene);
        }
    }

    private IEnumerator DelayedLoad()
    {
        yield return new WaitForSeconds(LoadProgress);
        SceneManager.sceneUnloaded += LoadScene;
        SceneManager.UnloadSceneAsync(currentSceneIndex);
    }

    private void LoadScene(Scene unloadedScene)
    {
        SceneManager.sceneUnloaded -= LoadScene;
        if (!SceneManager.GetSceneByBuildIndex(nextScene).isLoaded)
        {
            currentSceneIndex = nextScene;
            asyncProgress = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                SceneManager.SetActiveScene(scene);
            };
            sceneLoaded?.Fire();
        }
        else
        {
            Debug.LogWarning("Scene " + nextScene + " is already loaded");
        }
    }

    public void RestartScene(int sceneIndex)
    {
        Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
        if (scene.isLoaded)
        {
            SceneManager.sceneUnloaded += LoadScene;
            SceneManager.UnloadSceneAsync(sceneIndex);
        }
        else
        {
            LoadScene(scene);
        }
    }
}

[Serializable]
public struct SceneLoadTrigger
{
    public SEvent triggerEvent;
    public int sceneIndex;
}
