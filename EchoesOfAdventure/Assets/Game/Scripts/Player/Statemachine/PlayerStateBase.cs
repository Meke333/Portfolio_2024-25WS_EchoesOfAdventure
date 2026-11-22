using Base;
using System;
using Zenject;

namespace Player.Statemachine
{
    [System.Serializable]
    public class PlayerStateBase : StateBase
    {
        [Inject] protected PlayerStateData stateData;
        [Inject] public PlayerEvents playerEvents;
        [Inject] protected DiContainer diContainer;


        public virtual void SetStateType()
        {
            return;
        }

        public override void Enter()
        {
            base.Enter();
            SetStateType();

            //Subject
            playerEvents.State_DistributeUpdatedPlayerState?.Invoke(stateData.currentType);

            //Observer
            playerEvents.State_PlayerDeath += State_PlayerDeath;
        }

        public override void Exit()
        {
            base.Exit();

            //Observer
            playerEvents.State_PlayerDeath -= State_PlayerDeath;
        }

        protected virtual void State_PlayerDeath()
        {
            nextState = GetPlayerState_Dead();
            stage = EVENT.EXIT;
        }

        #region State-Retrieving
        protected Player.Statemachine.Idle GetPlayerState_Idle([Inject] Player.Statemachine.Idle idle = null)
        {
            if (idle == null)
                throw new InvalidOperationException("State could not be injected!");

            return idle;
        }
        protected Player.Statemachine.Dead GetPlayerState_Dead([Inject] Player.Statemachine.Dead dead = null)
        {
            if (dead == null)
                throw new InvalidOperationException("State could not be injected!");

            return dead;
        }
        protected Player.Statemachine.Jumping GetPlayerState_Jumping([Inject] Player.Statemachine.Jumping jumping = null)
        {
            if (jumping == null)
                throw new InvalidOperationException("State could not be injected!");

            return jumping;
        }
        protected Player.Statemachine.Falling GetPlayerState_Falling([Inject] Player.Statemachine.Falling falling = null)
        {
            if (falling == null)
                throw new InvalidOperationException("State could not be injected!");

            return falling;
        }
        protected Player.Statemachine.Grabbing GetPlayerState_Grabbing([Inject] Player.Statemachine.Grabbing grabbing = null)
        {
            if (grabbing == null)
                throw new InvalidOperationException("State could not be injected!");

            return grabbing;
        }
        protected Player.Statemachine.Climbing GetPlayerState_Climbing([Inject] Player.Statemachine.Climbing climbing = null)
        {
            if (climbing == null)
                throw new InvalidOperationException("State could not be injected!");

            return climbing;
        }
        #endregion
    }
}

