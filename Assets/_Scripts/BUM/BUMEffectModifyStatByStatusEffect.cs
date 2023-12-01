using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMEffectModifyStatByStatusEffect : BaseUpgradeMonoEffect
{
    public StatusEffectData Target;

    public StatDictionaryAllInOne ModifyStats;


    public override void Execute(Transform target)
    {
        if (target.TryGetComponent<BaseStats>(out var baseStats) && target.TryGetComponent<StatusEffectHandler>(out var statusEffectHandler))
        {
            baseStats.RemoveAllModifiersFromSource(this);
            if (statusEffectHandler.FindAppliedEffect(Target.ID) != null)
            {
                baseStats.AddBasicStatModifiers(ModifyStats, this);
            }
        }
    }
}
