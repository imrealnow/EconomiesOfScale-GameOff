using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScaleSFloatUpgrade", menuName = "Upgrades/ScaleSFloatUpgrade", order = 0)]
public class ScaleSFloatUpgrade : UpgradeSettings
{
    public float scaleAmount = 1.0f;
    public List<SFloat> floatToScale;

    protected override void ApplyUpgrade()
    {
        base.ApplyUpgrade();
        foreach (SFloat value in floatToScale)
        {
            value.Value *= scaleAmount;
        }
    }
}
