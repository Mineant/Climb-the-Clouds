using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////
/// TEMPLATE /// StatModifyEffectData
//////////////////////////////////

[CreateAssetMenu]
public class StatModifyEffectDataEffectData : StatusEffectData<StatModifyEffectDataEffectDataType, StatModifyEffectDataEffect>
{
    // Statis Data of the effect. Like the ID etc. Usually just a data container, no need to change anything.
}

[System.Serializable]
public class StatModifyEffectDataEffectDataType : StatusEffectDataType
{
    // The runtime data. Contains Max stacks, Duration etc. Should add variables that will be used by effect here. No Methods should be implemented here. They are implemented at StatModifyEffectDataEffect.
    [Header("Stat Modify")]
    public StatDictionaryAllInOne StatModifiers;
}

[System.Serializable]
public class StatModifyEffectDataEffect : StatusEffect<StatModifyEffectDataEffectDataType>
{
    // The implementation of the effect. No Parameters here. Only Methods.
    protected BaseStats _targetStats;

    protected override void OnApplied()
    {
        _targetStats = Target.GetComponent<BaseStats>();
        if (_targetStats != null)
        {
            _targetStats.AddBasicStatModifiers(DataType.StatModifiers, this);
        }
    }

    protected override void OnRemoved()
    {
        if (_targetStats != null)
        {
            _targetStats.RemoveAllModifiersFromSource(this);
        }
    }

    protected override void OnUpdate()
    {

    }
}

//////////////////////////////////
