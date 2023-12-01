using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BUMTriggerStatusEffectHandler : BaseUpgradeMonoTrigger
{
    public StatusEffectData Target;
    public bool TriggerOnApply;
    public bool TriggerOnRemove;
    public bool TriggerOnApplyToOtherPeople;
    private StatusEffectHandler _handler;

    [Header("Targets")]
    public bool SetOtherPeopleAsTarget;

    void Awake()
    {
        _handler = GetComponentInParent<StatusEffectHandler>();
    }

    private void OnApplyEffect(string obj)
    {
        if (TriggerOnApply && Target.ID == obj) Trigger();
    }

    private void OnRemoveEffect(string obj)
    {
        if (TriggerOnRemove && Target.ID == obj) Trigger();
    }

    private void OnAppliedEffectToTarget(string obj, Health health)
    {
        if (TriggerOnApplyToOtherPeople && Target.ID == obj)
        {
            if(SetOtherPeopleAsTarget) AddTarget(health.transform);
            Trigger();
        }
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        _handler.OnApplyEffect += OnApplyEffect;
        _handler.OnRemoveEffect += OnRemoveEffect;
        _handler.OnAppliedEffectToTarget += OnAppliedEffectToTarget;
    }




    protected override void OnDisable()
    {
        base.OnDisable();
        _handler.OnApplyEffect -= OnApplyEffect;
        _handler.OnRemoveEffect -= OnRemoveEffect;
        _handler.OnAppliedEffectToTarget -= OnAppliedEffectToTarget;
    }

}
