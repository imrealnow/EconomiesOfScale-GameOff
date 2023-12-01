using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "UpgradeSettings", menuName = "SO/UpgradeSettings")]
public class UpgradeSettings : ScriptableObject
{
    [Header("Info")]
    public string upgradeName;
    [TextArea] public string upgradeDescription;
    public Sprite upgradeIcon;

    [Header("Parameters")]
    public int cost;
    public bool infinitelyPurchasable;
    public int costIncreasePerPurchase;

    [Header("Prerequisites")]
    public List<UpgradeSettings> requiredUpgrades;

    [Header("References")]
    public UpgradesManager upgradesManager;
    public SInt playerGold;

    [Header("Events")]
    public SEvent purchaseTrigger;
    [Space] public UnityEvent applyUpgrade;

    private int purchaseCount = 0;
    private int dynamicCost;
    private bool isInitialised = false;

    public bool IsAvailable
    {
        get
        {
            if (requiredUpgrades != null && requiredUpgrades.Count > 0)
            {
                foreach (var upgrade in requiredUpgrades)
                {
                    if (!upgradesManager.IsUpgradePurchased(upgrade))
                        return false;
                }
            }
            if (purchaseCount > 0 && !infinitelyPurchasable)
                return false;
            return true;
        }
    }

    public bool HasBeenPurchased
    {
        get
        {
            return purchaseCount > 0;
        }
    }
    public bool PlayerCanAfford
    {
        get
        {
            if (!isInitialised)
                EnableUpgrade();
            return playerGold.Value >= dynamicCost;
        }
    }

    public int Cost
    {
        get
        {
            return dynamicCost;
        }
    }


    public void EnableUpgrade()
    {
        if (purchaseTrigger != null)
            purchaseTrigger.sharedEvent += Purchase;
        dynamicCost = cost;
        purchaseCount = 0;
        isInitialised = true;
    }

    public void DisableUpgrade()
    {
        if (purchaseTrigger != null)
            purchaseTrigger.sharedEvent -= Purchase;
    }

    public void Purchase()
    {
        if (!IsAvailable)
            return;
        if (!PlayerCanAfford)
            return;

        purchaseCount++;
        playerGold.Value -= cost;
        if (infinitelyPurchasable)
            dynamicCost += costIncreasePerPurchase;
        upgradesManager.NotifyUpgradePurchased();
        upgradesManager.AddPurchasedUpgrade(this);
        ApplyUpgrade();
    }

    [ContextMenu("Apply Upgrade")]
    protected virtual void ApplyUpgrade()
    {
        applyUpgrade?.Invoke();
    }
}
