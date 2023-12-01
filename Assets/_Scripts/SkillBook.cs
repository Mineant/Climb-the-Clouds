using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine;

[CreateAssetMenu]
public class SkillBook : BaseInventoryItem
{
    [Header("Skill Book")]
    public BasicStats BasicStats;
    public List<BaseUpgradeMono> SpecialEffectVariations;
    public BaseUpgradeMono SpecialEffect;

    public Rarity Rarity;
    public List<SkillType> RelatedSkillTypes;

    public bool IsInstance { get; protected set; }


    public SkillBook GetInstance()
    {
        if (IsInstance) Debug.LogError("Shouldnt instance twice.");

        var instance = Instantiate(this);
        instance.IsInstance = true;
        if (instance.SpecialEffectVariations.Count > 0)
        {
            instance.SpecialEffect = instance.SpecialEffectVariations.PickRandom<BaseUpgradeMono>();
        }

        return instance;
    }


    public int MaxSkillSlots
    {
        get
        {
            return (int)BasicStats.StatDictionaryAllInOne.BaseValueStatDictionary[Stat.SkillCapacity];
        }
    }


    public override int GetBuyCost()
    {
        return (int)((((int)Rarity + 1) * 60) * Mathf.Pow(1.5f, ((int)(Rarity))));
    }

    public override int GetSellRevenue()
    {
        return (int)(GetBuyCost() * 0.5f);
    }

}
