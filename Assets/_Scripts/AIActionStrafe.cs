using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    public class AIActionStrafe : AIAction
    {
        public enum StrafeMode { Left, Right, Random }
        public StrafeMode strafeMode;
        public float Offset;

        protected Vector3 _directionToTarget;
        protected CharacterMovement _characterMovement;
        protected Vector2 _movementVector;
        protected StrafeMode _strafeMode;

        public override void Initialization()
        {
            if (!ShouldInitialize) return;
            base.Initialization();
            _characterMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _strafeMode = strafeMode != StrafeMode.Random ? strafeMode : (Random.value > 0.5f) ? StrafeMode.Left : StrafeMode.Right;
        }

        public override void PerformAction()
        {
            Strafe();
        }

        protected virtual void Strafe()
        {
            if (_brain.Target == null)
            {
                return;
            }

            _directionToTarget = _brain.Target.position - this.transform.position;
            Vector3 directionRight = Vector3.Cross(_directionToTarget, Vector3.up).normalized; // 90 degrees to the right
            Vector3 directionLeft = -directionRight; // 90 degrees to the left
            Vector3 targetDirection = Vector3.zero;

            // Add strafing logic here
            switch (_strafeMode)
            {
                case StrafeMode.Left:
                    targetDirection = directionLeft;
                    break;
                case StrafeMode.Right:
                    targetDirection = directionRight;
                    break;
            }

            _movementVector.x = targetDirection.x;
            _movementVector.y = targetDirection.z;

            _characterMovement.SetMovement(_movementVector);
        }

        public override void OnExitState()
        {
            base.OnExitState();

            _characterMovement?.SetHorizontalMovement(0f);
            _characterMovement?.SetVerticalMovement(0f);
        }
    }
}
