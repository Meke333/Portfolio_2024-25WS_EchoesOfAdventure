using Base;
using NaughtyAttributes;
using Zenject;

namespace Player.Statemachine
{
    public class PlayerStateMachine : StatemachineBase<PlayerStateBase, PlayerStateData>
    {
        [Inject] protected PlayerBase player;
        [Inject] protected PlayerEvents playerEvents;
        [Inject] private DiContainer diContainer;

        [ShowNonSerializedField] private PlayerStateType stateType;

        [Inject]
        public void Inject(PlayerStateData data)
        {
            stateData = data;
        }


        #region Unity Methods
        private void Awake()
        {
            //To-Do: Fill in a concrete, implemented state
            currentState = diContainer.Resolve<Player.Statemachine.Idle>();

            playerEvents.General_ResetPlayer += ResetPlayer;  

            ToggleActivity(true);
        }

        protected override void Update()
        {
            base.Update();

            stateType = stateData.currentType;
        }

        private void OnDestroy()
        {
            playerEvents.General_ResetPlayer -= ResetPlayer;
        }

        #endregion

        private void ResetPlayer()
        {
            currentState.Exit();
            currentState = diContainer.Resolve<Player.Statemachine.Idle>();
        }
    }
}

