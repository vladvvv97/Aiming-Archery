using System;
using UnityEngine;

// Per-level data asset: arrow budgets by type + star thresholds (see SPEC.md 2.3).
[CreateAssetMenu(fileName = "LevelConfig", menuName = "Archery/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Serializable]
    public struct ArrowBudget
    {
        public ArrowTypeId type;
        public int count;
    }

    [Header("Identity")]
    public int levelId = 1;
    public string sceneName;

    [Header("Arrow budgets (only types with count > 0 are used)")]
    public ArrowBudget[] arrowBudgets = new ArrowBudget[] { new ArrowBudget { type = ArrowTypeId.Normal, count = 5 } };

    [Header("Stars (score = sum of remaining arrows on win)")]
    public int threeStarMin = 3;
    public int twoStarMin = 1;

    public int GetBudget(ArrowTypeId type)
    {
        foreach (var b in arrowBudgets)
        {
            if (b.type == type) return b.count;
        }
        return 0;
    }

    public int ComputeStars(int remainingScore)
    {
        if (remainingScore >= threeStarMin) return 3;
        if (remainingScore >= twoStarMin) return 2;
        return 1;
    }
}
