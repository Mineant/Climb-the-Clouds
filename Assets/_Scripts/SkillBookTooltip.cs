using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBookTooltip : Tooltip<SkillBookTooltipInfo>
{
    public SkillBookUIProduct SkillBookUIProduct;

    public override void Show(SkillBookTooltipInfo info)
    {
        base.Show(info);

        SkillBookUIProduct.Generate(new(info.SkillBook));
    }
}

public interface ISkillBookTooltipInfoProvider : ITooltipInfoProvider<SkillBookTooltipInfo> { }


[System.Serializable]
public class SkillBookTooltipInfo : TooltipInfo
{
    public SkillBook SkillBook;

    public SkillBookTooltipInfo(SkillBook skillBook)
    {
        SkillBook = skillBook;
    }

}