using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.TopDownEngine;
using Sirenix.OdinInspector;
// using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    public Transform EffectParticlePoint;
    public List<StatusEffect> AppliedStatusEffects;
    public Action<string, Health> OnAppliedEffectToTarget;
    private Health _health;
    private Dictionary<string, ParticleSystem> _runningParticleDict;
    public Action<string> OnApplyEffect;
    public Action<string> OnRemoveEffect;

    void Awake()
    {
        _health = GetComponent<Health>();
        _health.OnDeath += OnHealthDeath;
        AppliedStatusEffects = new();
        _runningParticleDict = new();
    }

    private void OnHealthDeath()
    {
        RemoveAllEffects();
    }

    private ParticleSystem CreateParticle(ParticleSystem particleSystem)
    {
        var particle = MonoBehaviour.Instantiate(particleSystem, EffectParticlePoint.transform);
        particle.transform.localPosition = UnityEngine.Vector3.zero;
        particle.Play();
        return particle;
    }

    public void ApplyEffect(StatusEffectField statusEffectField)
    {
        ApplyEffect(statusEffectField, null);
    }

    public void ApplyEffect(StatusEffectField statusEffectField, GameObject owner)
    {
        if (statusEffectField.CustomChance && UnityEngine.Random.Range(0f, 1f) > statusEffectField.Chance) return;

        if (statusEffectField.CustomDuration) ApplyEffect(statusEffectField.TargetStatusEffectData, owner, statusEffectField.Duration);
        else ApplyEffect(statusEffectField.TargetStatusEffectData, owner, -1f);
    }


    public void ApplyEffect(StatusEffectData statusEffectData, GameObject owner, float duration)
    {
        if (this.gameObject.activeInHierarchy == false) return;

        StatusEffect effectInstance = FindAppliedEffect(statusEffectData.ID);
        if (effectInstance != null)
        {
            // Update the effect time.
            float maxDuration = duration > 0f ? duration : effectInstance.Duration;
            effectInstance.CurrentDuration = Mathf.Max(effectInstance.CurrentDuration, maxDuration);
            effectInstance.Apply();
            return;
        }

        effectInstance = statusEffectData.GetEffect(_health, owner);
        if (duration > 0f) effectInstance.Duration = duration;
        effectInstance.OnRemove += OnEffectRemoved;
        AppliedStatusEffects.Add(effectInstance);

        if (statusEffectData.ApplyParticle != null) CreateParticle(statusEffectData.ApplyParticle);
        if (statusEffectData.RunningParticle != null) _runningParticleDict[statusEffectData.ID] = CreateParticle(statusEffectData.RunningParticle);
        if (OnApplyEffect != null) OnApplyEffect.Invoke(statusEffectData.ID);

        effectInstance.Apply();
    }


    public void RemoveEffect(string effectID)
    {
        StatusEffect statusEffect = FindAppliedEffect(effectID);
        if (statusEffect != null)
        {
            RemoveEffect(statusEffect);
        }
    }


    /// <summary>
    /// The Actual remove effect
    /// </summary>
    private void RemoveEffect(StatusEffect statusEffect)
    {
        AppliedStatusEffects.Remove(statusEffect);
        statusEffect.Remove();
        if (_runningParticleDict.TryGetValue(statusEffect.ID, out var particle))
        {
            GameObject.Destroy(particle.gameObject);
            _runningParticleDict.Remove(statusEffect.ID);
        }
        if (OnRemoveEffect != null) OnApplyEffect.Invoke(statusEffect.ID);
    }

    public void RemoveAllEffects()
    {
        int count = AppliedStatusEffects.Count;
        for (int i = 0; i < count; i++)
        {
            if (AppliedStatusEffects.Count == 0) break;

            RemoveEffect(AppliedStatusEffects[0]);
        }
        AppliedStatusEffects.Clear();
    }


    private void OnEffectRemoved(string effectID)
    {
        RemoveEffect(effectID);
    }

    public void AppliedEffectToTarget(string id, Health target)
    {
        if (OnAppliedEffectToTarget != null) OnAppliedEffectToTarget.Invoke(id, target);
    }

    public StatusEffect FindAppliedEffect(string effectID)
    {
        return AppliedStatusEffects.FirstOrDefault(s => s.ID == effectID);
    }

    void OnDisable()
    {
        RemoveAllEffects();
    }


}

[System.Serializable]
public class StatusEffectField
{
    public StatusEffectData TargetStatusEffectData;
    public bool CustomChance = false;

    [ShowIf(nameof(CustomChance))]
    public float Chance = 1f;
    public bool CustomDuration = false;

    [ShowIf(nameof(CustomDuration))]
    public float Duration = 1f;

    public bool CustomPotency;

    [ShowIf(nameof(CustomPotency))]
    public float Potency;
}
