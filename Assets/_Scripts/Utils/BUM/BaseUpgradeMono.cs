using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseUpgradeMono : MonoBehaviour
{
    // public List<ActionCardData> NewActionCards;
    // // public List<ActionCardStatModifier> ActionCardStatModifiers;
    // [Tooltip("New stats are provided by upgrade mono, not base upgrade, because it is more reliable, since if this mono behavior is removed, then it is guarateed that no stat will be added.")]
    // public List<StatField> NewStats;
    // public BaseUpgradeEffect[] UpgradeEffects { get; protected set; }
    [TextArea]
    public string Description;
    public BaseUpgradeMonoAbility[] Abilities { get; protected set; }
    public bool Initialized { get; protected set; }
    public GameObject Owner { get; protected set; }

    /// <summary>
    /// We can customize the owner as we like.
    /// </summary>
    /// <param name="owner"></param>
    public virtual void Initialize(GameObject owner = null)
    {
        Initialized = true;
        Owner = owner;
        Abilities = GetComponentsInChildren<BaseUpgradeMonoAbility>();

    }

    /// <summary>
    public virtual void Remove()
    {

    }

    public virtual void TriggerAllAbilities()
    {
        foreach (var ability in Abilities)
        {
            ability.Trigger();
        }
    }

    public string GetDescription()
    {
        return String.Join("\n", GetAbilities().Select(d => "· " + d.GetDescription()).Prepend(Description).Where(d => d.Length > 0));
    }

    private BaseUpgradeMonoAbility[] GetAbilities()
    {
        if(Abilities == null || Abilities.Length == 0) return GetComponentsInChildren<BaseUpgradeMonoAbility>();
        return Abilities;
    }
    // public IEnumerable<float> GetAdditionalAdditiveModifier(Stats stat)
    // {
    //     foreach (var item in NewStats)
    //         yield return item.AdditiveModifier;
    // }

    // public IEnumerable<float> GetAdditionalPercentageModifier(Stats stat0)
    // {
    //     foreach (var item in NewStats)
    //         yield return item.PercentageModifier;
    // }

}

