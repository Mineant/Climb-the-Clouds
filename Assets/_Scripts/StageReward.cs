using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kryz.CharacterStats.Examples;
using MoreMountains.InventoryEngine;
using UnityEngine;

[System.Serializable]
public class StageReward
{
    public StageRewardType RewardType;
    public List<BaseInventoryItem> RewardItems;
    public List<SkillType> RewardSkillTypes
    {
        get
        {
            List<SkillType> rewardSkillTypes = new();
            foreach (var item in RewardItems)
            {
                if (item is Skill skill) rewardSkillTypes.AddRange(skill.SkillTypes);
                else if (item is SkillBook skillBook) rewardSkillTypes.AddRange(skillBook.RelatedSkillTypes);
            }
            return rewardSkillTypes.Distinct().ToList();
        }
    }
}

public enum StageRewardType
{
    SkillAndSkillBooks, RandomSkillAndSkillBooks
}