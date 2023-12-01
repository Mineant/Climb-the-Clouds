using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BUMEffectStatusEffect : BaseUpgradeMonoEffect
{
    public List<StatusEffectField> ApplyEffects;
    public List<StatusEffectField> RemoveEffects;
    private GameObject _owner;


    void Awake()
    {
        _owner = GetComponentInParent<Character>()?.gameObject;
    }

    public override void Execute(Transform target)
    {
        if (target.TryGetComponent<StatusEffectHandler>(out var handler))
        {
            foreach (var effect in ApplyEffects)
            {
                handler.ApplyEffect(effect, _owner);
            }

            foreach (var effect in RemoveEffects)
            {
                handler.RemoveEffect(effect.TargetStatusEffectData.ID);
            }
        }
    }

}
