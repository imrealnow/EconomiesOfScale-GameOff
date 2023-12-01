using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreManager", menuName = "SO/Managers/ScoreManager", order = 1)]
public class ScoreManager : SManager
{
    public SInt score;

    public void ChangeScore(int amount)
    {
        score.Value += amount;
    }

    public void SetScore(int amount)
    {
        score.Value = amount;
    }
}
