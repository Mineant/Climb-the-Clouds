using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
public class BUMTriggerSkillEvent : BaseUpgradeMonoTrigger, MMEventListener<SkillEvent>
{
    public SkillEventType SkillEventType;
    public List<Skill> Skills;
    public List<SkillType> SkillTypes;
    public List<SkillBook> SkillBooks;
    public List<int> SkillIndexes;
    public bool CheckEmpowered;

    [EnableIf(nameof(CheckEmpowered))]
    public bool IsEmpowered;

    [Header("Other")]
    public bool SetWeaponAsTarget;


    public void OnMMEvent(SkillEvent eventType)
    {
        if (eventType.EventType != SkillEventType) return;
        if (Skills.Count > 0 && !Skills.Contains(eventType.Skill)) return;
        if (SkillTypes.Count > 0 && !SkillTypes.Any(t => eventType.Skill.SkillTypes.Contains(t))) return;
        if (SkillBooks.Count > 0 && !SkillBooks.Contains(eventType.SkillBook)) return;
        if (SkillIndexes.Count > 0)
        {
            if (!SkillIndexes.Contains(eventType.SkillIndex))
            {
                if (!SkillIndexes.Any(s => s < 0)) return;

                int capacity = (int)eventType.SkillBook.BasicStats.StatDictionaryAllInOne.BaseValueStatDictionary[Stat.SkillCapacity];
                if (!SkillIndexes.Where(s => s < 0).Select(s => capacity + s).Contains(eventType.SkillIndex)) return;
            }
        }
        if (CheckEmpowered && eventType.Empowered != IsEmpowered) return;

        if (SetWeaponAsTarget)
        {
            if (eventType.Weapon == null) Debug.LogWarning($"The weapon of {eventType} doesnt exist. Will not set weapon as target");
            else AddTarget(eventType.Weapon.transform);
        }

        Trigger();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening<SkillEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<SkillEvent>();
    }


}
