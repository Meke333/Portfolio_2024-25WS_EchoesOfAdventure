using Helper;
using Interface;
using NaughtyAttributes;
using ObjectPool;
using Player;
using UnityEngine;
using Zenject;

namespace Interactable
{
    [SelectionBase]
    public abstract class InteractableBase : PoolObjectBase, IInteractable
    {
        [Inject] protected InteractableEvents _interactableEvents;

        [SerializeField, ReadOnly] private InteractableState _state = InteractableState.Normal;
        protected InteractableType interactableType;
        [SerializeField] protected CharacterMask characterMask;
        protected InteractableMask interactableMask { get; private set; }


        public InteractableState State
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;

                _state = value;
                //SolveStateBehaviour();
            }
        }

        private void Awake()
        {
            SetInteractableType();

            if (State == 0)
                State = InteractableState.Normal;

            SolveStateBehaviour();
        }

        private void OnValidate()
        {
            GetInteractableMask();
        }

        private InteractableMask GetInteractableMask()
        {
            return (InteractableMask)(0x1 << (int)interactableType);
        }

        public void SolveStateBehaviour()
        {
            ResetObject();

            switch (State)
            {
                case InteractableState.Normal:
                    NormalBehaviourState();
                    break;
                case InteractableState.Destroyed:
                    DestroyedBehaviourState();
                    break;
                case InteractableState.Burned:
                    BurnedBehaviourState();
                    break;
                case InteractableState.Sprouted:
                    SproutedBehaviourState();
                    break;
                case InteractableState.Stone:
                    StoneBehaviourState();
                    break;
                default:
                    break;
            }
        }

        protected abstract void NormalBehaviourState();
        protected abstract void DestroyedBehaviourState();
        protected abstract void BurnedBehaviourState();
        protected abstract void SproutedBehaviourState();
        protected abstract void StoneBehaviourState();

        #region IInteractable

        protected abstract void SetInteractableType();
        public InteractableType GetInteractableType()
        {
            return interactableType;
        }

        public abstract void StartInteracting();

        public abstract void StopInteracting();

        public abstract bool IsInteractionAllowed(CharacterType type);
        #endregion
    }

}
