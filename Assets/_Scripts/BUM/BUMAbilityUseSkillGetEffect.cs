using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMAbilityUseSkillGetEffect : BaseUpgradeMonoAbility
{
    public BUMTriggerSkillEvent BUMTriggerSkillEvent { get { return GetComponent<BUMTriggerSkillEvent>(); } }
    public BUMEffectStatusEffect BUMEffectStatusEffect { get { return GetComponent<BUMEffectStatusEffect>(); } }
    public override string GetDescription()
    {
        List<string> statusEffects = new();

        foreach (var effectField in BUMEffectStatusEffect.ApplyEffects)
        {
            var msg = "";
            msg += effectField.CustomChance ? (effectField.Chance * 100) + "%" : "100%";
            msg += $" chance to get {effectField.TargetStatusEffectData.Name}";
            statusEffects.Add(msg);
        }

        var empoweredText = "";
        if (BUMTriggerSkillEvent.CheckEmpowered && BUMTriggerSkillEvent.IsEmpowered) empoweredText = " if empowered";

        return $"{String.Join(", ", BUMTriggerSkillEvent.SkillTypes)} skill have {String.Join(", ", statusEffects)} when being used{empoweredText}.";
    }
}
