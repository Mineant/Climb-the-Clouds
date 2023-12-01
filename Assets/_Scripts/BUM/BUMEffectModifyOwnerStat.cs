using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BUMEffectModifyOwnerStat : BaseUpgradeMonoEffect
{
    public StatDictionaryAllInOne ModifyStats;
    protected BaseStats _ownerStat;

    void Awake()
    {
        _ownerStat = GetComponentInParent<Character>().GetComponent<BaseStats>();
    }

    public override void Execute(Transform target)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _ownerStat.AddBasicStatModifiers(ModifyStats, this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _ownerStat.RemoveAllModifiersFromSource(this);
    }
}
