using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BaseCharacterMovement : CharacterMovement
{
    protected Object _movementLocker;

    public void ForbiddenMovement(Object locker)
    {
        _movementLocker = locker;
        MovementForbidden = true;
        _characterMovement.SetMovement(Vector2.zero);
    }

    public void UnforbiddenMovement(Object unlocker)
    {
        if (MovementForbidden == false) return;
        if (_movementLocker == null)
        {
            MovementForbidden = false;
            return;
        }

        if (_movementLocker == unlocker)
        {
            MovementForbidden = false;
            _movementLocker = null;
            return;
        }
    }
}
