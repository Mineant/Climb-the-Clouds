using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BUMFilterMonoOwner))]
[RequireComponent(typeof(BUMEffectWuXiaWeaponStatModifier))]
public class BUMAbilityAuraSkillModifyStat : BaseUpgradeMonoAbility
{
    public override string GetDescription()
    {
        return StatHelper.GetAllInOneStatString(GetComponent<BUMEffectWuXiaWeaponStatModifier>().StatDictionaryAllInOne, ", ");
    }
}
