using System;
using UnityEngine;

namespace Player.Statemachine
{
    public class Falling : PlayerStateBase
    {
        float _currentVelocity;
        bool _isGrounded; 

        public override void SetStateType()
        {
            stateData.currentType = PlayerStateType.Falling;
        }

        public override void Enter()
        {
            base.Enter();
            playerEvents.Movement_OnJump += Movement_OnJump;
            playerEvents.Movement_OnGroundChecking += Movement_OnGroundChecking;
            playerEvents.State_RequestingClimbState += State_RequestingClimbState;
            playerEvents.Movement_VelocityTransmitY += Movement_VelocityTransmit;
        }

        public override void Update()
        {
            base.Update();
            ResolveFallingState();
        }

        public override void Exit()
        {
            base.Exit();
            playerEvents.Movement_OnJump -= Movement_OnJump;
            playerEvents.Movement_OnGroundChecking -= Movement_OnGroundChecking;
            playerEvents.State_RequestingClimbState -= State_RequestingClimbState;
            playerEvents.Movement_VelocityTransmitY -= Movement_VelocityTransmit;
        }

        private void ResolveFallingState()
        {
            if (!_isGrounded || Mathf.Abs(_currentVelocity) > 0.1f)
            {
                return;
            }

            nextState = diContainer.Resolve<Idle>();
            stage = EVENT.EXIT;
        }

        private void Movement_OnJump()
        {
            nextState = diContainer.Resolve<Player.Statemachine.Jumping>();
            stage = EVENT.EXIT;
        }

        private void Movement_VelocityTransmit(float obj)
        {
            _currentVelocity = obj;
        }

        private void Movement_OnGroundChecking(bool isGrounded, float arg2)
        {
            _isGrounded = isGrounded;
        }

        private void State_RequestingClimbState(bool isClimbing)
        {
            if (!isClimbing)
                return;

            nextState = diContainer.Resolve<Climbing>();
            stage = EVENT.EXIT;
        }
    }
}

