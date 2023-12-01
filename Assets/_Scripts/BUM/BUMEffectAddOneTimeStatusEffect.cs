using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMEffectAddOneTimeStatusEffect : BaseUpgradeMonoEffect
{
    public List<StatusEffectField> DamageOnTouchOneTimeEffects;
    public override void Execute(Transform target)
    {
        if (target.TryGetComponent<BaseWeapon>(out var weapon))
        {
            foreach (var effect in DamageOnTouchOneTimeEffects)
            {
                weapon.AddOneTimeDamageApplyStatusEffect(effect);
            }
        }
    }

}
