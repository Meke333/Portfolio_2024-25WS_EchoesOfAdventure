using Ability;
using Events;
using Helper;
using System;
using UnityEngine;
using Zenject;

namespace Player
{
    /// <summary>
    /// Triggers Player specific abilities
    /// </summary>
    public class PlayerAbility : PlayerSystem
    {
        [Inject] private PlayerAbilityConfiguration _abilityConfig;

        [SerializeField] private AbilityBase _ability;

        [Header("configurations")]
        [SerializeField] private bool _usesLookDirection;
        [SerializeField] private bool _uses4Directions;
        [SerializeField] private bool _uses8Directions;
        [SerializeField] private float _offset = 1;

        private Vector2 _lastDirection;
        private CharacterType characterType;

        private PlayerStateType _stateType;

        private float _time = 0f;
        private bool _timerActive;

        #region Unity Methods

        private void OnEnable()
        {
            _playerEvents.Input_OnUseAbilityInput += UseAbilityInput;
            _playerEvents.Input_OnMoveInput += MoveInput;

            _playerEvents.State_DistributeUpdatedPlayerState += UpdatePlayerState;

        }
        private void Update()
        {
            if (!_timerActive)
                return;

            _time += Time.deltaTime;

            if (_time < _abilityConfig.AbilityCooldown)
                return;

            _time = 0f;
            _timerActive = false;
        }

        private void MoveInput(Vector2 vector)
        {
            if (vector.magnitude <= 0)
                return;

            _lastDirection = vector;
        }

        private void OnDisable()
        {
            _playerEvents.Input_OnUseAbilityInput -= UseAbilityInput;
            _playerEvents.Input_OnMoveInput -= MoveInput;

            _playerEvents.State_DistributeUpdatedPlayerState -= UpdatePlayerState;

        }

        #endregion

        private void UseAbilityInput(bool obj)
        {


            switch (_stateType)
            {
                case PlayerStateType.Dead:
                case PlayerStateType.Grabbing:
                case PlayerStateType.Climbing:
                    return;
            }

            if (obj)
            {
                if (_timerActive)
                    return;

                Vector2 manipulated = new Vector2(0, 0);
                if (_usesLookDirection)
                {
                    manipulated = new Vector2(_player.lookDirection, 0);

                }

                else if (_uses4Directions)
                {
                    manipulated = DetectDirectionHelper.Get4DirectionVector(_lastDirection);
                    
                    if (manipulated.magnitude <= 0)
                    {
                        manipulated = new Vector2(_player.lookDirection, 0);
                    }
                }

                else if (_uses8Directions)
                {
                    manipulated = DetectDirectionHelper.Get8DirectionVector(_lastDirection);

                    if (manipulated.magnitude <= 0)
                    {
                        manipulated = new Vector2(_player.lookDirection, 0);
                    }
                }

                else
                {
                    manipulated = new Vector2(0, 0);
                }

                _ability.UseAbility(_offset * manipulated);
                _playerEvents.Ability_UseAbility?.Invoke(true);

                _timerActive = true;


            }
            else
            {
                _ability.StopAbility();
                _playerEvents.Ability_UseAbility?.Invoke(false);
            }

        }

        private void UpdatePlayerState(PlayerStateType type)
        {
            _stateType = type;
        }
    }
}

