using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMFilterMonoOwner : BaseUpgradeMonoFilter
{
    public override void FilterTargets(List<Transform> targetsCache)
    {
        var mono = GetComponentInParent<BaseUpgradeMono>();
        if (mono == null)
        {
            Debug.LogWarning("There is no mono in parent. Cannot get the owner from it.");
            return;
        }


        targetsCache.Add(mono.Owner.transform);
    }

}
