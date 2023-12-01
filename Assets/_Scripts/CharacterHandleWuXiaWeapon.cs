using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterHandleWuXiaWeapon : CharacterHandleWeapon
{
    public override void ShootStart()
    {
        // if the Shoot action is enabled in the permissions, we continue, if not we do nothing.  If the player is dead we do nothing.
        if (!AbilityAuthorized
            || (CurrentWeapon == null)
            || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
        {
            return;
        }

        //  if we've decided to buffer input, and if the weapon is in use right now
        if (BufferInput && (CurrentWeapon.WeaponState.CurrentState != Weapon.WeaponStates.WeaponIdle))
        {
            // if we're not already buffering, or if each new input extends the buffer, we turn our buffering state to true
            ExtendBuffer();
        }

        if (BufferInput && RequiresPerfectTile && (_characterGridMovement != null))
        {
            if (!_characterGridMovement.PerfectTile)
            {
                ExtendBuffer();
                return;
            }
            else
            {
                _buffering = false;
            }
        }
        PlayAbilityStartFeedbacks();
        // CurrentWeapon.WeaponInputStart();

        ((WuXiaWeapon)CurrentWeapon).WuXiaWeapoonInputStart(Input.GetMouseButton(1));
    }
}
