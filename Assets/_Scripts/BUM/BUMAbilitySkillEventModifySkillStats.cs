using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMAbilitySkillEventModifySkillStats : BaseUpgradeMonoAbility
{
    public BUMTriggerSkillEvent BUMTriggerSkillEvent { get { return GetComponent<BUMTriggerSkillEvent>(); } }
    public BUMEffectWuXiaWeaponStatModifier BUMEffectWuXiaWeaponStatModifier { get { return GetComponent<BUMEffectWuXiaWeaponStatModifier>(); } }
    public override string GetDescription()
    {
        var skillIndexMsg = "";
        if (BUMTriggerSkillEvent.SkillIndexes.Count > 0)
        {
            List<string> messages = new();
            foreach (var index in BUMTriggerSkillEvent.SkillIndexes)
            {
                if (index == -1) messages.Add("last");
                else messages.Add(Helpers.GetIndicatedNumber(index));
            }
            skillIndexMsg = $" if in the {String.Join(", ", messages)} position";
        }

        var empowerMsg = "";
        if (BUMTriggerSkillEvent.CheckEmpowered && BUMTriggerSkillEvent.IsEmpowered) empowerMsg = " when empowered";
        
        return $"{String.Join(", ", BUMTriggerSkillEvent.SkillTypes)} skill's {StatHelper.GetAllInOneStatString(GetComponent<BUMEffectWuXiaWeaponStatModifier>().StatDictionaryAllInOne, ", ")}{empowerMsg}{skillIndexMsg}.";
    }
}
