using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

//////////////////////////////////
/// TEMPLATE /// OnDamagedApplyStatusEffectEffectData
//////////////////////////////////

[CreateAssetMenu]
public class OnDamagedApplyStatusEffectEffectDataEffectData : StatusEffectData<OnDamagedApplyStatusEffectEffectDataEffectDataType, OnDamagedApplyStatusEffectEffectDataEffect>
{
    // Statis Data of the effect. Like the ID etc. Usually just a data container, no need to change anything.
}

[System.Serializable]
public class OnDamagedApplyStatusEffectEffectDataEffectDataType : StatusEffectDataType
{
    // The runtime data. Contains Max stacks, Duration etc. Should add variables that will be used by effect here. No Methods should be implemented here. They are implemented at OnDamagedApplyStatusEffectEffectDataEffect.
    [Header("Status Effect")]
    public StatusEffectField ApplyStatusEffectField;
}

[System.Serializable]
public class OnDamagedApplyStatusEffectEffectDataEffect : StatusEffect<OnDamagedApplyStatusEffectEffectDataEffectDataType>
{
    // The implementation of the effect. No Parameters here. Only Methods.
    private Health _health;
    private StatusEffectHandler _statusEffectHandler;

    protected override void OnApplied()
    {
        _health = Target.GetComponent<Health>();
        _statusEffectHandler = Target.GetComponent<StatusEffectHandler>();
        if (_health != null)
        {
            _health.OnHit += OnHit;
        }
    }


    protected override void OnRemoved()
    {
        if (_health != null)
        {
            _health.OnHit -= OnHit;
        }
    }

    protected override void OnUpdate()
    {

    }


    private void OnHit()
    {
        if (_statusEffectHandler != null)
        {
            _statusEffectHandler.ApplyEffect(DataType.ApplyStatusEffectField, Owner);
        }
    }

}

//////////////////////////////////
