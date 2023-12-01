using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIBinder : MonoBehaviour
{
    public UpgradeSettings upgrade;
    public TextMeshProUGUI upgradeTitle;
    public TextMeshProUGUI upgradeCost;
    public Image upgradeIcon;
    public Button upgradeButton;
    public UpgradesManager upgradesManager;

    private void OnEnable()
    {
        if (upgrade != null)
            BindUpgrade(upgrade);
    }

    [ContextMenu("Bind Upgrade")]
    public void BindUpgrade(UpgradeSettings upgrade)
    {
        this.upgrade = upgrade;
        upgradeTitle.text = upgrade.upgradeName;
        upgradeCost.text = upgrade.Cost.ToString();
        upgradeIcon.sprite = upgrade.upgradeIcon;
        if (!upgrade.PlayerCanAfford || !upgrade.IsAvailable)
        {
            upgradeButton.interactable = false;
        }
        else
        {
            upgradeButton.interactable = true;
        }
    }

    public void PurchaseUpgrade()
    {
        if (upgrade.PlayerCanAfford)
        {

            StartCoroutine(DelayedUpgradePurchase());
        }
    }

    private IEnumerator DelayedUpgradePurchase()
    {
        upgradesManager.NotifyUpgradePurchased();
        while (Time.timeScale == 0)
        {
            yield return null;
        }
        upgrade.Purchase();
    }
}
