using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "UpgradesManager", menuName = "SO/Managers/UpgradesManager")]
public class UpgradesManager : SManager
{
    public List<UpgradeSettings> allUpgrades = new List<UpgradeSettings>();
    public SInt playerGold;
    public UnityEvent OnUpgradePurchased;
    public SEvent resetEvent;

    private List<UpgradeSettings> purchasedUpgrades = new List<UpgradeSettings>();

    public List<UpgradeSettings> AvailableUpgrades
    {
        get
        {
            return allUpgrades.Where(upgrade => upgrade.IsAvailable).ToList();
        }
    }

    public bool IsUpgradePurchased(UpgradeSettings upgrade)
    {
        return purchasedUpgrades.Contains(upgrade);
    }

    public override void OnEnabled()
    {
        base.OnEnabled();
        purchasedUpgrades.Clear();
        resetEvent.sharedEvent += Reset;
        foreach (var upgrade in allUpgrades)
        {
            upgrade.EnableUpgrade();
        }
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        resetEvent.sharedEvent -= Reset;
        foreach (var upgrade in allUpgrades)
        {
            upgrade.DisableUpgrade();
        }
    }

    public void NotifyUpgradePurchased()
    {
        if (OnUpgradePurchased != null)
            OnUpgradePurchased.Invoke();
    }

    public void AddPurchasedUpgrade(UpgradeSettings upgrade)
    {
        if (purchasedUpgrades.Contains(upgrade))
            return;

        purchasedUpgrades.Add(upgrade);
    }

    public void Reset()
    {
        purchasedUpgrades.Clear();
    }
}
