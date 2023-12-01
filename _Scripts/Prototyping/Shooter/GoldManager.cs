using UnityEngine;
using UnityEngine.Events;

public class GoldManager : MonoBehaviour
{
    public int startAmount = 500;
    public IntReference gold;
    public UnityEvent onAmountChanged;

    private void Start()
    {
        gold.Value = startAmount;
    }

    public void ChangeByAmount(int amount)
    {
        gold.Value = Mathf.Max(0, amount);
        if (onAmountChanged != null) onAmountChanged.Invoke();
    }
}
