using UnityEngine;

public class PoolContainerSingleton : MonoBehaviour
{
    private static PoolContainerSingleton _instance;
    public static PoolContainerSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PoolContainerSingleton>();
                if (_instance == null)
                {
                    GameObject container = new GameObject("PoolContainerSingleton");
                    _instance = container.AddComponent<PoolContainerSingleton>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
}
