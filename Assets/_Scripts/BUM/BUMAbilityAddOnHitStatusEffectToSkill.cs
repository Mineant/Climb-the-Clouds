using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMAbilityAddOnHitStatusEffectToSkill : BaseUpgradeMonoAbility
{
    public BUMTriggerSkillEvent BUMTriggerSkillEvent { get { return GetComponent<BUMTriggerSkillEvent>(); } }
    public BUMEffectAddOneTimeStatusEffect BUMEffectAddOneTimeStatusEffect { get { return GetComponent<BUMEffectAddOneTimeStatusEffect>(); } }
    public override string GetDescription()
    {
        List<string> statusEffects = new();

        foreach (var effectField in BUMEffectAddOneTimeStatusEffect.DamageOnTouchOneTimeEffects)
        {
            var msg = "";
            msg += effectField.CustomChance ? (effectField.Chance * 100) + "%" : "100%";
            msg += $" chance to apply {effectField.TargetStatusEffectData.Name}";
            statusEffects.Add(msg);
        }

        return $"{String.Join(", ", BUMTriggerSkillEvent.SkillTypes)} skill have {String.Join(", ", statusEffects)} on hit.";
    }
}
