using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using UnityEngine;

//////////////////////////////////
/// TEMPLATE /// SkillEventWeaponStatModifyEffectData
//////////////////////////////////

[CreateAssetMenu]
public class SkillEventWeaponStatModifyEffectDataEffectData : StatusEffectData<SkillEventWeaponStatModifyEffectDataEffectDataType, SkillEventWeaponStatModifyEffectDataEffect>
{
    // Statis Data of the effect. Like the ID etc. Usually just a data container, no need to change anything.
}

[System.Serializable]
public class SkillEventWeaponStatModifyEffectDataEffectDataType : StatusEffectDataType
{
    // The runtime data. Contains Max stacks, Duration etc. Should add variables that will be used by effect here. No Methods should be implemented here. They are implemented at SkillEventWeaponStatModifyEffectDataEffect.
    [Header("Weapon Skill Stat")]
    public StatDictionaryAllInOne StatModifiers;

    [Header("Conditions")]
    public Skill TargetSkill;
    public SkillBook TargetSkillBook;
    public List<SkillType> TargetSkillTypes;
    public bool CheckEmpowered;
    public bool IsEmpowered;

}

[System.Serializable]
public class SkillEventWeaponStatModifyEffectDataEffect : StatusEffect<SkillEventWeaponStatModifyEffectDataEffectDataType>
{
    // The implementation of the effect. No Parameters here. Only Methods.
    protected CharacterWuXiaCombat _combat;

    protected override void OnApplied()
    {
        _combat = Target.GetComponent<CharacterWuXiaCombat>();
        if (_combat != null)
        {
            _combat.OnUseSkill += OnUseSkill;
        }
    }


    protected override void OnRemoved()
    {
        if (_combat != null)
        {
            _combat.OnUseSkill -= OnUseSkill;
        }
    }

    protected override void OnUpdate()
    {

    }


    private void OnUseSkill(SkillEvent skillEvent)
    {
        if (skillEvent.EventType != SkillEventType.UseSkill) return;
        if (DataType.TargetSkill != null && DataType.TargetSkill != skillEvent.Skill) return;
        if (DataType.TargetSkillBook != null && DataType.TargetSkillBook != skillEvent.SkillBook) return;
        if (DataType.TargetSkillTypes.Count > 0 && !DataType.TargetSkillTypes.Intersect(skillEvent.Skill.SkillTypes).Any()) return;
        if (DataType.CheckEmpowered && DataType.IsEmpowered != skillEvent.Empowered) return;

        skillEvent.Weapon.AddOneTimeStatModifier(DataType.StatModifiers);
    }

}

//////////////////////////////////
