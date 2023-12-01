using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using Unity.VisualScripting;
using UnityEngine;
using static Helpers;

public abstract class StatusEffectData : ScriptableObject
{
    // This is the Container of the status effect
    [ScriptableObjectId]
    public string ID;

    public string Name;

    [TextArea]
    public string Description;

    [Header("Particle Effects")]
    public ParticleSystem ApplyParticle;
    public ParticleSystem RunningParticle;

    public abstract StatusEffect GetEffect(Health target, GameObject ownerz);
}

public abstract class StatusEffectDataType
{
    // This Contains the runtime values of the status effect
    public int MaxStacks = 1;
    public bool HasDuration;
    public float Duration = 60;

    // [Header("Effects")]
    // public ParticleSystem ApplyParticle;
    // public ParticleSystem RemoveParticle;
    // public ParticleSystem RunningParticle;

}

[System.Serializable]
public abstract class StatusEffect
{
    // The runtime of the status effect.
    public string ID;
    public Health Target;
    public GameObject Owner;
    public int MaxStacks;
    public int CurrentStacks;
    public float Potency;
    public float SecondaryPotency;
    public float Duration;
    public float CurrentDuration;

    public Action<string> OnApply;
    public Action<string> OnRemove;

    public abstract void Apply();
    public abstract void Remove();
}


public abstract class StatusEffectData<TDataType, TEffect> : StatusEffectData where TDataType : StatusEffectDataType where TEffect : StatusEffect<TDataType>, new()
{
    public TDataType Type;
    public override StatusEffect GetEffect(Health target, GameObject owner)
    {
        return new TEffect() { ID = ID, Target = target, DataType = Type, Owner = owner };
    }
}

public abstract class StatusEffect<TDataType> : StatusEffect where TDataType : StatusEffectDataType
{

    public TDataType DataType;

    protected bool _applied;
    protected Coroutine _effectCoroutine;
    // protected ParticleSystem _particle;

    public override void Apply()
    {
        // Check if effect is already applied
        if (_applied)
        {
            // Update the stats.
            CurrentStacks = Math.Min(MaxStacks, CurrentStacks + 1);
            return;
        }

        Duration = DataType.HasDuration ? DataType.Duration : Mathf.Infinity;
        CurrentDuration = Duration;
        MaxStacks = DataType.MaxStacks;
        CurrentStacks = 1;

        _applied = true;
        OnApplied();
        if (OnApply != null) OnApply.Invoke(ID);
        if (Owner.TryGetComponent<StatusEffectHandler>(out var statusEffectHandler)) statusEffectHandler.AppliedEffectToTarget(ID, Target);

        _effectCoroutine = Target.StartCoroutine(_EffectCoroutine());
    }

    public override void Remove()
    {
        if (!_applied) return;

        if (_effectCoroutine != null) Target.StopCoroutine(_effectCoroutine);

        _applied = false;
        OnRemoved();
        if (OnRemove != null) OnRemove.Invoke(ID);
    }


    protected virtual IEnumerator _EffectCoroutine()
    {
        do
        {
            CurrentDuration -= Time.deltaTime;
            OnUpdate();
            yield return true;
        } while (CurrentDuration > 0);

        Remove();
    }

    protected ParticleSystem CreateParticle(ParticleSystem particleSystem)
    {
        var particle = MonoBehaviour.Instantiate(particleSystem, Target.transform);
        particle.transform.localPosition = UnityEngine.Vector3.zero;
        particle.Play();
        return particle;
    }

    protected virtual void OnApplied()
    {
        // To Be Implemented
    }

    protected virtual void OnRemoved()
    {
        // To Be Implemented
    }

    protected virtual void OnUpdate()
    {
        // To be Implemented
    }

}

//////////////////////////////////
/// TEMPLATE /// TYPE
//////////////////////////////////

// [CreateAssetMenu]
// public class TYPEEffectData : StatusEffectData<TYPEEffectDataType, TYPEEffect>
// {
//     // Statis Data of the effect. Like the ID etc. Usually just a data container, no need to change anything.
// }

// [System.Serializable]
// public class TYPEEffectDataType : StatusEffectDataType
// {
//     // The runtime data. Contains Max stacks, Duration etc. Should add variables that will be used by effect here. No Methods should be implemented here. They are implemented at TYPEEffect.
// }

// [System.Serializable]
// public class TYPEEffect : StatusEffect<TYPEEffectDataType>
// {
//     // The implementation of the effect. No Parameters here. Only Methods.

//     protected override void OnApplied()
//     {
//        
//     }

//     protected override void OnRemoved()
//     {
//         
//     }

//     protected override void OnUpdate()
//     {
//         
//     }
// }

//////////////////////////////////
