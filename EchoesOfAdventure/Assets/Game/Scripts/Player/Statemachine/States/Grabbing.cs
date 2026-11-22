namespace Player.Statemachine
{
    public class Grabbing : PlayerStateBase
    {
        public override void SetStateType()
        {
            stateData.currentType = PlayerStateType.Grabbing;
        }

        public override void Enter()
        {
            base.Enter();
            playerEvents.State_RequestingGrabState += State_RequestingGrabState;
            playerEvents.Movement_OnGroundChecking += Movement_OnGroundChecking;
        }

        public override void Exit()
        {
            base.Exit();
            playerEvents.State_RequestingGrabState -= State_RequestingGrabState;
            playerEvents.Movement_OnGroundChecking -= Movement_OnGroundChecking;
        }

        private void State_RequestingGrabState(bool isGrabbing)
        {
            if (isGrabbing)
                return;

            nextState = diContainer.Resolve<Player.Statemachine.Idle>();
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

