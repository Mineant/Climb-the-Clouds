using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BUMFilterHasHealth : BaseUpgradeMonoFilter
{
    public override void FilterTargets(List<Transform> targetsCache)
    {
        targetsCache.RemoveAll(t => t.GetComponent<Health>() == null);
    }
}
