using System.Collections;
using System.Collections.Generic;
using Kryz.CharacterStats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatValueUIProduct : Product<StatValueUIProductArgs>
{
    [System.Serializable]
    public class StatBarSetting
    {
        public Stat Stat;
        public float HighestValue;
        public Color Color;
        public bool Invert;

        public StatBarSetting(Stat stat, float highestValue, Color color, bool invert = false)
        {
            this.Stat = stat;
            this.HighestValue = highestValue;
            this.Color = color;
            this.Invert = invert;
        }

    }

    public TMP_Text StatText;
    public TMP_Text ValueText;
    public Image Bar;
    public static List<StatBarSetting> StatBarSettings = new List<StatBarSetting>()
    {
        new(Stat.Health,100f,ColorSettings.SeaGreen),
        new(Stat.QiCost,80f,ColorSettings.SkyBlue),
        new(Stat.AuraSkillCost,3,ColorSettings.Orange),
        new(Stat.AuraRegen,80f,ColorSettings.Orange),
        new(Stat.PhysicalDamage,80f,ColorSettings.IndianRed1),
        new(Stat.SpellDamage,80f,ColorSettings.IndianRed1),
        new(Stat.SkillCapacity,9,ColorSettings.DarkGreen),
        new(Stat.QiRegen,100f,ColorSettings.SkyBlue),
        new(Stat.MaxQi,250f,ColorSettings.SkyBlue),
        new(Stat.RecoveryTime,2f,ColorSettings.BurlyWood),
        new(Stat.SkillTime,2f,ColorSettings.BurlyWood),

        new(Stat.BurstLength,3f,ColorSettings.DarkSeaGreen),
        new(Stat.ProjectilesPerShot,3f,ColorSettings.DarkSeaGreen),
        new(Stat.Spread,40f,ColorSettings.DarkSeaGreen),
        new(Stat.Range,3f,ColorSettings.DarkSeaGreen),
        new(Stat.Recoil,8f,ColorSettings.DarkSeaGreen),
    };

    public override void Generate(StatValueUIProductArgs productArgs)
    {
        base.Generate(productArgs);

        StatText.text = StatHelper.ToString(productArgs.Stat);

        if (productArgs.IsBaseValue)
        {
            ValueText.text = productArgs.Value.ToString("0.#");
        }
        else
        {
            string a = "";
            switch (productArgs.StatModType)
            {
                case (StatModType.Flat):
                    a = $"+{productArgs.Value.ToString("0.##")}";
                    break;
                case (StatModType.PercentAdd):
                    a = $"+{(productArgs.Value * 100).ToString("0.##")}%";
                    break;
                case (StatModType.PercentMult):
                    a = $"x{(productArgs.Value * 100).ToString("0.##")}%";
                    break;
                default:
                    a = $"ggggggggggggggggggggggggggggggggggggggggggggg";
                    break;
            }
            ValueText.text = a;
        }

        if (Bar != null)
        {
            var settings = StatBarSettings.Find(s => s.Stat == productArgs.Stat);
            Bar.fillAmount = Mathf.Clamp(productArgs.Value / settings.HighestValue, 0f, 1f);
            Bar.color = settings.Color;
        }
    }
}

public class StatValueUIProductArgs : ProductArgs
{
    public Stat Stat;
    public StatModType StatModType;
    public float Value;
    // public StatModifier StatModifier;
    public bool IsBaseValue;

    public StatValueUIProductArgs(Stat stat, float baseValue)
    {
        Stat = stat;
        Value = baseValue;
        IsBaseValue = true;
    }

    public StatValueUIProductArgs(Stat stat, StatModType statModType, float modValue)
    {
        Stat = stat;
        StatModType = statModType;
        Value = modValue;
        IsBaseValue = false;
    }

    public StatValueUIProductArgs(Stat stat, StatModifier modifier)
    {
        Stat = stat;
        StatModType = modifier.Type;
        Value = modifier.Value;
        IsBaseValue = false;
    }

}