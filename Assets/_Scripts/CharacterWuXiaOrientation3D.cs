using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterWuXiaOrientation3D : CharacterOrientation3D
{
    protected CharacterWuXiaCombat _characterWuXiaCombat;

    protected override void Initialization()
    {
        base.Initialization();
        _characterWuXiaCombat = _character.FindAbility<CharacterWuXiaCombat>();
    }

    protected override void RotateToFaceWeaponDirection()
    {
        if (_characterWuXiaCombat == null) return;

        _newWeaponQuaternion = Quaternion.identity;
        _weaponRotationDirection = Vector3.zero;
        _shouldRotateTowardsWeapon = false;

        // if we're not supposed to face our direction, we do nothing and exit
        if (!ShouldRotateToFaceWeaponDirection) { return; }
        if ((RotationMode != RotationModes.WeaponDirection) && (RotationMode != RotationModes.Both)) { return; }
        if (_characterWuXiaCombat == null) { return; }
        if (_characterWuXiaCombat.IsUsingSkill == false) { return; }

        _shouldRotateTowardsWeapon = true;

        _rotationDirection = _characterWuXiaCombat.CurrentWeapanAim.CurrentAim.normalized;

        if (LockVerticalRotation)
        {
            _rotationDirection.y = 0;
        }

        _weaponRotationDirection = _rotationDirection;

        MMDebug.DebugDrawArrow(this.transform.position, _rotationDirection, Color.red);

        // if the rotation mode is instant, we simply rotate to face our direction
        if (WeaponRotationSpeed == RotationSpeeds.Instant)
        {
            if (_rotationDirection != Vector3.zero)
            {
                _newWeaponQuaternion = Quaternion.LookRotation(_rotationDirection);
            }
        }

        // if the rotation mode is smooth, we lerp towards our direction
        if (WeaponRotationSpeed == RotationSpeeds.Smooth)
        {
            if (_rotationDirection != Vector3.zero)
            {
                _tmpRotation = Quaternion.LookRotation(_rotationDirection);
                _newWeaponQuaternion = Quaternion.Slerp(WeaponRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceWeaponDirectionSpeed);
            }
        }

        // if the rotation mode is smooth, we lerp towards our direction even if the input has been released
        if (WeaponRotationSpeed == RotationSpeeds.SmoothAbsolute)
        {
            if (_rotationDirection.normalized.magnitude >= AbsoluteThresholdWeapon)
            {
                _lastMovement = _rotationDirection;
            }
            if (_lastMovement != Vector3.zero)
            {
                _tmpRotation = Quaternion.LookRotation(_lastMovement);
                _newWeaponQuaternion = Quaternion.Slerp(WeaponRotatingModel.transform.rotation, _tmpRotation, Time.deltaTime * RotateToFaceWeaponDirectionSpeed);
            }
        }
    }
}
