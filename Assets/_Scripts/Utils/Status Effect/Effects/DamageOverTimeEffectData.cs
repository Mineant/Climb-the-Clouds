using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

//////////////////////////////////
/// TEMPLATE /// DamageOverTime
//////////////////////////////////

[CreateAssetMenu]
public class DamageOverTimeEffectData : StatusEffectData<DamageOverTimeEffectDataType, DamageOverTimeEffect>
{
    // Statis Data of the effect. Like the ID etc. Usually just a data container, no need to change anything.
}

[System.Serializable]
public class DamageOverTimeEffectDataType : StatusEffectDataType
{
    // The runtime data. Contains Max stacks, Duration etc. Should add variables that will be used by effect here. No Methods should be implemented here. They are implemented at DamageOverTimeEffect.
    [Header("DamageOverTime")]
    public float Damage = 1f;
    public float Interval = 1f;

    public bool UseDamageModifierStat;
    public Stat DamageModifierStat;
    // public MMF_Player OnDamageFeedback;
}

[System.Serializable]
public class DamageOverTimeEffect : StatusEffect<DamageOverTimeEffectDataType>
{
    // The implementation of the effect. No Parameters here. Only Methods.

    public float CurrentInterval;
    protected float _damage;

    protected override void OnApplied()
    {
        base.OnApplied();

        _damage = DataType.Damage;
        if (DataType.UseDamageModifierStat && Owner.TryGetComponent<BaseStats>(out var baseStats))
        {
            baseStats.SetBaseValue(DataType.DamageModifierStat, _damage);
            _damage = baseStats.GetStat(DataType.DamageModifierStat);
        }
    }

    protected override void OnRemoved()
    {
        base.OnRemoved();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        CurrentInterval -= Time.deltaTime;

        if (CurrentInterval <= 0f)
        {
            Target.Damage(_damage, null, 0f, 0f, Vector3.zero, null);
            // if (DataType.OnDamageFeedback != null) DataType.OnDamageFeedback.PlayFeedbacks(Target.transform.position);

            CurrentInterval = DataType.Interval;
        }
    }
}

//////////////////////////////////