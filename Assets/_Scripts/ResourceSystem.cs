using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialogueQuests;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceSystem : MMPersistentSingleton<ResourceSystem>
{
    public StatDamageTypeDictionary StatDamageTypeTable;
    public List<Skill> Skills;
    public List<SkillBook> SkillBooks;

    // public Actor PlayerActor;
    // public List<SkillType> InGameSkillTypes { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        // foreach (var item in Skills) if (item.SkillTypes.Count == 0) Debug.LogWarning($"{item.ItemName} Skill no skill types. Will not appear in stage rewards;");
        // foreach (var item in SkillBooks) if (item.RelatedSkillTypes.Count == 0) Debug.LogWarning($"{item.ItemName} Skill Book no skill types. Will not appear in stage rewards;");

        // InGameSkillTypes = Skills.SelectMany(s => s.SkillTypes).Concat(SkillBooks.SelectMany(s => s.RelatedSkillTypes)).Distinct().ToList();
    }


    public List<BaseInventoryItem> GetSkillAndSkillBooksCustomChance(Rarity targetRarity)
    {
        if (Random.Range(0f, 1f) < 0.75f)
        {
            return Skills.Where(s => s.Rarity == targetRarity).Cast<BaseInventoryItem>().ToList();
        }
        else
        {
            return SkillBooks.Where(s => s.Rarity == targetRarity).Cast<BaseInventoryItem>().ToList();
        }
    }

    public List<BaseInventoryItem> GetSkillAndSkillBooks(Rarity targetRarity)
    {
        return Skills.Where(s => s.Rarity == targetRarity).Cast<BaseInventoryItem>().Concat(ResourceSystem.Instance.SkillBooks.Where(s => s.Rarity == targetRarity)).ToList();
    }
}

[System.Serializable]
public class StatDamageTypeDictionary : UnitySerializedDictionary<Stat, DamageType> { }
