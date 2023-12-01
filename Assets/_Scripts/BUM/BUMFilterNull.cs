using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMFilterNull : BaseUpgradeMonoFilter
{
    public override void FilterTargets(List<Transform> targetsCache)
    {
        targetsCache.Add(null);
    }
}
