using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AIActionStopMove : AIAction
{
    private CharacterMovement _characterMovement;


    public override void Initialization()
    {
        base.Initialization();
        _characterMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
    }

    public override void PerformAction()
    {
        _characterMovement.SetMovement(Vector3.zero);
    }


}
