using Ability;
using Helper;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;


namespace Player
{
    [SelectionBase]
    public class PlayerBase : MonoBehaviour
    {
        [field: SerializeField] public CharacterType CharacterType { get; private set; }
        public CharacterMask CharacterMask { get; private set; }

        [field: SerializeField, ReadOnly] public PlayerStateType PlayerState { get; private set; }


        [Inject] protected PlayerEvents playerEvents;

        public int lookDirection { get; private set; }

        private void Start()
        {
            CharacterMask = LayerAndMaskHelper.GetMaskFromType<CharacterMask, CharacterType>(CharacterType);
        }

        private void OnEnable()
        {
            playerEvents.LookDirection_OnLookingDirectionChange += SetCharacterLookDirection;
            playerEvents.State_DistributeUpdatedPlayerState += GetPlayerState;


        }

        private void OnDisable()
        {
            playerEvents.LookDirection_OnLookingDirectionChange -= SetCharacterLookDirection;
            playerEvents.State_DistributeUpdatedPlayerState -= GetPlayerState;

        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            CharacterMask = LayerAndMaskHelper.GetMaskFromType<CharacterMask, CharacterType>(CharacterType);
        }
#endif

        private void GetPlayerState(PlayerStateType type)
        {
            PlayerState = type;
        }

        private void SetCharacterLookDirection(int direction)
        {
            switch (PlayerState)
            {
                case PlayerStateType.Dead:
                case PlayerStateType.Grabbing:
                case PlayerStateType.Climbing:
                    return;
            }

            lookDirection = direction;
        }

        public void ResetCharacter()
        {
            playerEvents.LookDirection_OnLookingDirectionChange?.Invoke(1);
            playerEvents.General_ResetPlayer?.Invoke();
        }
    }
}

