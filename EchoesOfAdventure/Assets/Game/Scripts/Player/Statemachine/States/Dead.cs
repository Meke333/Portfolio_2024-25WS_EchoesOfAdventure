namespace Player.Statemachine
{
    public class Dead : PlayerStateBase
    {
        public override void SetStateType()
        {
            stateData.currentType = PlayerStateType.Dead;
        }

        protected override void State_PlayerDeath()
        {
            return;
        }
    }
}

