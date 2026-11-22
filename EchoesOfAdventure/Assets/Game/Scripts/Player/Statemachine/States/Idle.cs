using Player;
using System;
using UnityEngine;
using Zenject;

namespace Player.Statemachine
{
    //To-Do: Fill in Idle State
    public class Idle : PlayerStateBase
    {

        public override void SetStateType()
        {
            stateData.currentType = PlayerStateType.Idle;
        }

        public override void Enter()
        {
            base.Enter();
            playerEvents.Movement_OnJump += Movement_OnJump;
            playerEvents.Movement_OnGroundChecking += Movement_OnGroundChecking;
            playerEvents.State_RequestingClimbState += State_RequestingClimbState;
            playerEvents.State_RequestingGrabState += State_RequestingGrabState;
        }

        public override void Exit()
        {
            base.Exit();
            playerEvents.Movement_OnJump -= Movement_OnJump;
            playerEvents.Movement_OnGroundChecking -= Movement_OnGroundChecking;
            playerEvents.State_RequestingClimbState -= State_RequestingClimbState;
            playerEvents.State_RequestingGrabState -= State_RequestingGrabState;
        }

        private void State_RequestingGrabState(bool obj)
        {
            nextState = diContainer.Resolve<Player.Statemachine.Grabbing>();
            stage = EVENT.EXIT;
        }

        private void State_RequestingClimbState(bool obj)
        {
            nextState = diContainer.Resolve<Player.Statemachine.Climbing>();
            stage = EVENT.EXIT;
        }

        private void Movement_OnJump()
        {
            nextState = diContainer.Resolve<Player.Statemachine.Jumping>();
            stage = EVENT.EXIT;
        }

        private void Movement_OnGroundChecking(bool isGrounded, float arg2)
        {
            if (isGrounded)
                return;

            nextState = diContainer.Resolve<Player.Statemachine.Falling>();
            stage = EVENT.EXIT;
        }

    }
}

