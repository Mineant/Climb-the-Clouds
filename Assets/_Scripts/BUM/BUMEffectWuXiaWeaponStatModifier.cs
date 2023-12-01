using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BUMEffectWuXiaWeaponStatModifier : BaseUpgradeMonoEffect
{
    public StatDictionaryAllInOne StatDictionaryAllInOne;

    public override void Execute(Transform target)
    {
        if (target.TryGetComponent<WuXiaWeapon>(out var wuXiaWeapon))
        {
            wuXiaWeapon.AddOneTimeStatModifier(StatDictionaryAllInOne);
            // StartCoroutine(_ModifyWeaponStatUntilEnd(wuXiaWeapon));
        }
        else
        {
            Debug.LogWarning($"Dont have wuxia weapon on target [{target.gameObject.name}]. Cannot modify stat.");
        }
    }

    // private IEnumerator _ModifyWeaponStatUntilEnd(WuXiaWeapon wuXiaWeapon)
    // {
    //     if (wuXiaWeapon.WeaponState.CurrentState != Weapon.WeaponStates.WeaponIdle)
    //     {
    //         Debug.LogWarning($"The weapon [{wuXiaWeapon.name}] Must be idle for this bum effect [{this.gameObject.name}] to work.");
    //         yield break;
    //     }

    //     wuXiaWeapon.WeaponState.OnStateChange += OnStateChange;

    //     string key = Helpers.GetUniqueID();
    //     wuXiaWeapon.BaseStats.AddBasicStatModifiers(StatDictionaryAllInOne, key);

    //     bool hasEnded = false;
    //     while (!hasEnded) yield return true;

    //     wuXiaWeapon.BaseStats.RemoveAllModifiersFromSource(key);
    //     wuXiaWeapon.WeaponState.OnStateChange -= OnStateChange;

    //     void OnStateChange()
    //     {
    //         if (wuXiaWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponStop)
    //             hasEnded = true;
    //     }
    // }
}
