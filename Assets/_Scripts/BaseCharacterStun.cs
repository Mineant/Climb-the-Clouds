using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BaseCharacterStun : CharacterStun
{
    private BaseWeaponHandler _baseWeaponHandler;


    protected override void Initialization()
    {
        base.Initialization();
        _baseWeaponHandler = GetComponent<BaseWeaponHandler>();
    }
    public override void Stun()
    {
        if (!AbilityPermitted) return;
        
        base.Stun();
        if (_baseWeaponHandler != null)
        {
            _baseWeaponHandler.StopAllShooting();
        }
    }
}
