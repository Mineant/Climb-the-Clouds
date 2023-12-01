using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BUMFilterFindTargetRadius : BaseUpgradeMonoFilter
{
    public float Radius = 3f;
    public LayerMask TargetLayerMask;

    public override void FilterTargets(List<Transform> targetsCache)
    {
        var colliders = Physics.OverlapSphere(this.transform.position, Radius, TargetLayerMask);
        targetsCache.AddRange(colliders.Select(c => c.transform));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Vector3.zero, Radius);
    }
}
