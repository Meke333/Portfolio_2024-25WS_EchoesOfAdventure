using UnityEngine;

namespace Player.Statemachine
{
    public class Jumping : PlayerStateBase
    {
        float _currentVelocity;
        bool _isGrounded;

        public override void SetStateType()
        {
            stateData.currentType = PlayerStateType.Jumping;
        }

        public override void Enter()
        {
            base.Enter();
            playerEvents.Input_OnJumpInput += Input_OnJumpInput;
            playerEvents.Movement_OnGroundChecking += Movement_OnGroundChecking;
            playerEvents.Movement_VelocityTransmitY += Movement_VelocityTransmit;
            playerEvents.State_RequestingClimbState += State_RequestingClimbState;
        }

        public override void Update()
        {
            base.Update();
            ResolveFallingState();
        }

        public override void Exit()
        {
            base.Exit();
            playerEvents.Input_OnJumpInput -= Input_OnJumpInput;
            playerEvents.Movement_OnGroundChecking -= Movement_OnGroundChecking;
            playerEvents.Movement_VelocityTransmitY -= Movement_VelocityTransmit;
            playerEvents.State_RequestingClimbState -= State_RequestingClimbState;
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

        private void Movement_VelocityTransmit(float obj)
        {
            _currentVelocity = obj;
        }

        private void Movement_OnGroundChecking(bool isGrounded, float arg2)
        {
            _isGrounded = isGrounded;
        }

        private void Input_OnJumpInput(bool isButtonDown)
        {
            if (isButtonDown)
                return;

            nextState = diContainer.Resolve<Player.Statemachine.Falling>();
            stage = EVENT.EXIT;
        }

        private void State_RequestingClimbState(bool isClimbing)
        {
            if (!isClimbing)
                return;

            nextState = diContainer.Resolve<Player.Statemachine.Climbing>();
            stage = EVENT.EXIT;
        }
    }
}

