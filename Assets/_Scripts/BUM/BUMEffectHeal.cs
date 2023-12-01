using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BUMEffectHeal : BaseUpgradeMonoEffect
{
    public enum HealMode
    {
        Abosolute,
        PercentageOnMaxHealth,
    }
    public HealMode Mode;
    public int Value;

    public override void Execute(Transform target)
    {
        if (target.TryGetComponent<Health>(out var health))
        {
            if (Mode == HealMode.Abosolute)
            {
                health.ReceiveHealth(Value, null);
            }
            else if (Mode == HealMode.PercentageOnMaxHealth)
            {
                health.ReceiveHealth(health.MaximumHealth * Value, null);
            }

        }
    }
}
