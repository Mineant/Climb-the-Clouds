using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BUMTriggerSkillEvent))]
public class BUMAbilityIfSkillIndexIsAThenDoSth : BaseUpgradeMonoAbility
{
    public override string GetDescription()
    {
        var triggerSkillEvent = GetComponent<BUMTriggerSkillEvent>();
        string skillIndexes = String.Join(", ", triggerSkillEvent.SkillIndexes.Select(a => Helpers.GetIndicatedNumber(a)));
        string skillTypes = String.Join(", ", triggerSkillEvent.SkillTypes);

        if (triggerSkillEvent.SkillTypes.Count == 0)
        {
            return $"The {skillIndexes} skill(s) are ...";
        }
        
        return $"If the {skillIndexes} skill(s) is/are a {skillTypes}, ...";
    }
}
