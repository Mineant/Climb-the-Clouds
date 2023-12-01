using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMTriggerWuXiaCombatEvents : BaseUpgradeMonoTrigger
{
    public bool TriggerOnRecover;
    private CharacterWuXiaCombat _combat;


    void Awake()
    {
        _combat = GetComponentInParent<CharacterWuXiaCombat>();
    }
    private void OnRecover()
    {
        if (TriggerOnRecover) Trigger();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _combat.OnRecover += OnRecover;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _combat.OnRecover -= OnRecover;
    }


}
