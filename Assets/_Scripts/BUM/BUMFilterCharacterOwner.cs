using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BUMFilterCharacterOwner : BaseUpgradeMonoFilter
{
    public override void FilterTargets(List<Transform> targetsCache)
    {
        var parent = GetComponentInParent<Character>();
        if (parent != null)
        {
            targetsCache.Add(parent.transform);
        }
    }
}
