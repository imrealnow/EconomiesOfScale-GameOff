using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ManagerRegistry : MonoBehaviour
{
    [SerializeField] private List<SManager> registeredManagers = new List<SManager>();

    private static ManagerRegistry _instance;

    public static ManagerRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<ManagerRegistry>();
            }

            return _instance;
        }
    }

    public void RegisterManager(SManager manager)
    {
        if (!registeredManagers.Contains(manager))
            registeredManagers.Add(manager);
    }

    public void DeregisterManager(SManager manager)
    {
        if (registeredManagers.Contains(manager))
            registeredManagers.Remove(manager);
    }

    public T GetManager<T>() where T : SManager
    {
        foreach (SManager manager in registeredManagers)
        {
            if (manager.GetType() == typeof(T))
                return (T)manager;
        }
        return null;
    }

    public async Task<T> GetManagerAsync<T>() where T : SManager
    {
        var timeoutTask = Task.Delay(5000);  // timeout delay

        while (true)
        {
            foreach (SManager manager in registeredManagers)
            {
                if (manager.GetType() == typeof(T))
                    return (T)manager;  // found, return manager
            }

            // check if timed out
            if (await Task.WhenAny(Task.Delay(100), timeoutTask) == timeoutTask)
                return null;  // timed out, return null

            await Task.Delay(100);  // delay before next attempt
        }
    }
}
