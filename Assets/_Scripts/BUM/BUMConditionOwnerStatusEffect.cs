using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMConditionOwnerStatusEffect : BaseUpgradeMonoCondition
{
    public StatusEffectData Target;
    private StatusEffectHandler _handler;


    void Awake()
    {
        _handler = GetComponentInParent<StatusEffectHandler>();
    }
    public override bool Evaluate()
    {
        if (_handler.FindAppliedEffect(Target.ID) == null) return false;
        return true;
    }
}
