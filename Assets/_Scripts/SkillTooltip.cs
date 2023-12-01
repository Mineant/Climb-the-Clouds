using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTooltip : Tooltip<SkillTooltipInfo>
{
    public SkillUIProduct SkillUIProduct;

    public override void Show(SkillTooltipInfo info)
    {
        base.Show(info);

        SkillUIProduct.Generate(new(info.Skill));
    }
}

public interface ISkillTooltipInfoProvider : ITooltipInfoProvider<SkillTooltipInfo> { }


[System.Serializable]
public  class SkillTooltipInfo : TooltipInfo
{
    public Skill Skill;

    public SkillTooltipInfo(Skill skill)
    {
        Skill = skill;
    }

}