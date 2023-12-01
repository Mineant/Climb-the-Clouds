using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BUMEffectDamage : BaseUpgradeMonoEffect
{
    public enum DamageType { Absolute, MaxHealthPercentage, CurrentHealthPercentage }

    public DamageType Type;

    public float Value;

    public override void Execute(Transform target)
    {
        if (target.TryGetComponent<Health>(out var health))
        {
            float damage = 0f;
            switch (Type)
            {
                case (DamageType.Absolute):
                    damage = Value;
                    break;
                case (DamageType.MaxHealthPercentage):
                    damage = health.MaximumHealth * Value;
                    break;
                case (DamageType.CurrentHealthPercentage):
                    damage = health.CurrentHealth * Value;
                    break;
            }
            health.Damage(damage, null, 0f, 0f, Vector3.zero, null);
        }
    }

}
