using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class StatHelper
{
    public static string ToString(Stat stat)
    {
        switch (stat)
        {
            case (Stat.BurstLength):
                return "Use Times";
            case (Stat.ProjectilesPerShot):
                return "Projectiles";
            case (Stat.Spread):
                return "Spread";
            case (Stat.QiCost):
                return "Energy Cost";
            case (Stat.AuraSkillCost):
                return "Aura Skill cost";
            case (Stat.TrueDamage):
                return "True Damage";
            case (Stat.QiRegen):
                return "Energy Regen";
            case (Stat.MaxQi):
                return "Max Energy";
            case (Stat.AuraRegen):
                return "Aura Regen";
            case (Stat.PhysicalDamage):
                return "Physical Damage";
            case (Stat.SpellDamage):
                return "Magic Damage";
            case (Stat.Range):
                return "Range";
            case (Stat.RecoveryTime):
                return "Recovery Time";
            case (Stat.SkillTime):
                return "Skill Time";
            case (Stat.SkillCapacity):
                return "Skill Capacity";
            case (Stat.Recoil):
                return "Recoil";
            case (Stat.BurnDamage):
                return "Burn Damage";
            case (Stat.LightningStrikeDamage):
                return "Lightning-Strike Damage";

            default:
                return $"ToString for {stat} is not yet defined.";
        }
    }

    public static string GetAllInOneStatString(StatDictionaryAllInOne statDictionaryAllInOne, string seperator)
    {
        List<string> messages = new();

        foreach (var statValue in statDictionaryAllInOne.BaseValueStatDictionary)
            messages.Add($"{StatHelper.ToString(statValue.Key)} = {statValue.Value.ToString()}");

        foreach (var statValue in statDictionaryAllInOne.FlatModifierStatDictionary)
            messages.Add($"{StatHelper.ToString(statValue.Key)} {statValue.Value.ToStringWithSign()}");

        foreach (var statValue in statDictionaryAllInOne.PercentAddModifierStatDictionary)
            messages.Add($"{StatHelper.ToString(statValue.Key)} {(statValue.Value * 100).ToStringWithSign()}%");

        foreach (var statValue in statDictionaryAllInOne.PercentMultModifierStatDictionary)
            messages.Add($"{StatHelper.ToString(statValue.Key)} {(statValue.Value * 100).ToStringWithSign()}%");

        return String.Join(seperator, messages);
    }

    public static string GetRarityString(Rarity rarity)
    {
        switch (rarity)
        {
            case (Rarity.Common):
                return $"<color=#{ColorSettings.LimeGreen.ToHexString()}>{rarity}</color>";
            case (Rarity.Uncommon):
                return $"<color=#{ColorSettings.SkyBlue.ToHexString()}>{rarity}</color>";
            case (Rarity.Rare):
                return $"<color=#{ColorSettings.MediumOrchid.ToHexString()}>{rarity}</color>";
            case (Rarity.Legendary):
                return $"<color=#{ColorSettings.DarkOrange.ToHexString()}>{rarity}</color>";
        }

        return rarity.ToString();
    }
}

[System.Serializable]
public class BaseMultiplierVariableStat
{
    public float BaseValue;
    public float Multiplier;

    public BaseMultiplierVariableStat(float baseValue, float multiplier)
    {
        BaseValue = baseValue;
        Multiplier = multiplier;
    }

    public float GetStat(float variable) => BaseValue + Multiplier * variable;
}
