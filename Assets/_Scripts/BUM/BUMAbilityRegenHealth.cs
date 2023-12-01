using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BUMFilterCharacterOwner))]
[RequireComponent(typeof(BUMEffectHeal))]
public class BUMAbilityRegenHealth : BaseUpgradeMonoAbility
{
    public override string GetDescription()
    {
        return $"Heal {GetComponent<BUMEffectHeal>().Value}HP";
    }
}
