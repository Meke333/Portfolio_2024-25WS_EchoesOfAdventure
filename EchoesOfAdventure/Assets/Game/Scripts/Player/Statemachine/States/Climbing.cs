namespace Player.Statemachine
{
    public class Climbing : PlayerStateBase
    {
        private bool _isOnGround;

        public override void SetStateType()
        {
            stateData.currentType = PlayerStateType.Climbing;
        }

        public override void Enter()
        {
            base.Enter();
            playerEvents.State_RequestingClimbState += State_RequestingClimbState;
            playerEvents.Movement_OnJump += Movement_OnJump;
            playerEvents.Movement_OnGroundChecking += Movement_OnGroundChecking;
        }

        public override void Exit()
        {
            base.Exit();
            playerEvents.State_RequestingClimbState -= State_RequestingClimbState;
            playerEvents.Movement_OnJump += Movement_OnJump;
            playerEvents.Movement_OnGroundChecking -= Movement_OnGroundChecking;
        }

        private void State_RequestingClimbState(bool isClimbing)
        {
            if (isClimbing)
                return;

            if (_isOnGround)
                nextState = diContainer.Resolve<Idle>();
            else
                nextState = diContainer.Resolve<Falling>();
            stage = EVENT.EXIT;
        }

        private void Movement_OnJump()
        {
            nextState = diContainer.Resolve<Jumping>();
            stage = EVENT.EXIT;
        }

        private void Movement_OnGroundChecking(bool onGround, float arg2)
        {
            _isOnGround = onGround;
        }
    } 
}

