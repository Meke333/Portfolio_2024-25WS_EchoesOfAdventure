using Interface;
using NaughtyAttributes;
using ObjectPool;
using System;
using UnityEngine;
using Zenject;

namespace Echo
{
    [SelectionBase]
    public abstract class EchoBase : PoolObjectBase
    {
        [Inject] protected EchoEvents echoEvents;
        [SerializeField, ReadOnly] private EchoState _state = EchoState.Normal;

        [field: Header("VFX")]
        [field: SerializeField] protected GameObject vfxGameObject { get; private set; }

        public EchoState State
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;

                _state = value;
            }
        }

        private void Awake()
        {
            if (State == 0)
                State = EchoState.Normal;

            SolveStateBehaviour();
        }

        private void OnEnable()
        {
            if(vfxGameObject != null)
                vfxGameObject.SetActive(true);
        }

        public void SolveStateBehaviour()
        {
            switch (State)
            {
                case EchoState.Normal:
                    NormalStateBehaviour();
                    break;
                case EchoState.Destroyed:
                    DestroyedStateBehaviour();
                    break;
                case EchoState.Burned:
                    BurnedStateBehaviour();
                    break;
                default:
                    break;
            }
        }

        protected abstract void NormalStateBehaviour();
        protected abstract void DestroyedStateBehaviour();
        protected abstract void BurnedStateBehaviour();
    }

    public enum EchoState
    {
        Normal = 1,
        Destroyed = 2,
        Burned = 3,
    }
}




