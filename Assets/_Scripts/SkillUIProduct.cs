using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIProduct : Product<SkillUIProductArgs>, ISkillTooltipInfoProvider
{
    [Header("Information")]
    public Image IconImage;
    public TMP_Text NameText;
    public TMP_Text ShortDescriptionText;
    public TMP_Text RarityText;
    public TMP_Text SkillTypeText;
    public TMP_Text AuraSkillDesciptionText;
    public StatValueUIContainer StatValueUIContainer;

    [Header("Interactive")]
    public GameObject HighlighedOverlay;

    public Skill Skill { get; protected set; }



    public override void Generate(SkillUIProductArgs productArgs)
    {
        base.Generate(productArgs);

        Skill = productArgs.Skill;
        if (IconImage) IconImage.sprite = Skill.Icon;
        if (NameText) NameText.text = Skill.ItemName;
        if (ShortDescriptionText) ShortDescriptionText.text = Skill.ShortDescription;
        if (RarityText) RarityText.text = StatHelper.GetRarityString(Skill.Rarity);
        if (SkillTypeText) SkillTypeText.text = String.Join(", ", Skill.SkillTypes);
        if (AuraSkillDesciptionText && Skill.WuXiaWeapon.AuraSkillMono != null) AuraSkillDesciptionText.text = $"<b><color=#F9A31B>Aura Skill:</b></color>\n{Skill.WuXiaWeapon.AuraSkillMono.GetDescription()}";
        if (StatValueUIContainer) StatValueUIContainer.Generate(Skill.WuXiaWeapon.BaseStats.BasicStats, Skill.SkillDefaultParameters);


        SetHighlight(false);
    }



    public override void Hide()
    {
        base.Hide();
        Skill = null;
    }

    public void SetHighlight(bool value) { if (HighlighedOverlay != null) HighlighedOverlay.SetActive(value); }


    public SkillTooltipInfo GetTooltipInfo()
    {
        return new(Skill);
    }
}

public class SkillUIProductArgs : ProductArgs
{
    public Skill Skill;

    public SkillUIProductArgs(Skill skill)
    {
        this.Skill = skill;
    }

}