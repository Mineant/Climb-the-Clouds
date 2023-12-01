using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUMEffectQiResource : BaseUpgradeMonoEffect
{
    public enum ChangeMode { Absolute, Percentage }

    public ChangeMode Mode;
    public float QiChange;
    public float AuraChange;


    public override void Execute(Transform target)
    {
        if (target.TryGetComponent<CharacterWuXiaResource>(out var component))
        {
            if (Mode == ChangeMode.Absolute)
            {
                if (QiChange != 0f)
                {
                    component.QiBar.ChangeValue(QiChange);
                }

                if (AuraChange != 0f)
                {
                    component.AuraBar.ChangeValue(AuraChange);
                }
            }
            else
            {
                if (QiChange != 0f)
                {
                    component.QiBar.ChangeValue(component.QiBar.MaxValue * QiChange);
                }

                if (AuraChange != 0f)
                {
                    component.AuraBar.ChangeValue(component.AuraBar.MaxValue * AuraChange);
                }
            }

        }
    }
}
