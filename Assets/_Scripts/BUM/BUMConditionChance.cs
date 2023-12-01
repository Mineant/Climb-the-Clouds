using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMConditionChance : BaseUpgradeMonoCondition
{
    public float Chance;
    public override bool Evaluate()
    {
        return UnityEngine.Random.Range(0f, 1f) <= Chance;
    }
}
