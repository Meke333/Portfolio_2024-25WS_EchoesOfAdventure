using Events;
using Helper;
using NaughtyAttributes;
using Player;
using System;
using UnityEngine;
using Zenject;

namespace Player
{
    //Handles Feedback for Animator & Sound
    public class PlayerFeedback : PlayerSystem
    {
        [Inject] private ProjectEvents _projectEvents;

        private PlayerStateType _playerStateType;
        private CharacterType _characterType;

        bool _activateHigherJump;
        bool _isOnGround;
        bool _isMovingHorizontal;
        bool _isInWater;

        private Vector2 _inputDirection;
        private Vector2 _rigidBodyDirection;

        [SerializeField, ReadOnly] bool isCli;


        //Liams stopping animator when climbing
        [SerializeField] private Animator _animator;
        [SerializeField] private Animator _animatorClimb;


        [SerializeField] private Transform _landingVFXTransform;

        private void Start()
        {
            _characterType = _player.CharacterType;
        }

        //CLIMB
        //PUSH BOX
        //IF CHARACTER IS IN WATER

        private void OnEnable()
        {
            _playerEvents.Input_OnMoveInput += OnMoveInput;
            _playerEvents.State_DistributeUpdatedPlayerState += State_DistributeUpdatedPlayerState;
            _playerEvents.Movement_ActivateHigherJump += Movement_ActivateHigherJump;
            _playerEvents.Movement_OnMovingHorizontal += Movement_OnMovingHorizontal;
            _playerEvents.Movement_VelocityTransmit += Movement_VelocityTransmit;
            _playerEvents.Movement_OnGroundChecking += Movement_OnGroundChecking;
            _playerEvents.Movement_IsClimbing += Movement_IsClimbing;
            _playerEvents.Ability_UseAbility += Ability_UseAbility;
            _playerEvents.Ability_Success += Ability_Success;
        }


        private void OnDisable()
        {
            ResetHandler();

            _playerEvents.State_DistributeUpdatedPlayerState -= State_DistributeUpdatedPlayerState;
            _playerEvents.Movement_ActivateHigherJump -= Movement_ActivateHigherJump;
            _playerEvents.Movement_OnMovingHorizontal -= Movement_OnMovingHorizontal;
            _playerEvents.Movement_VelocityTransmit -= Movement_VelocityTransmit;
            _playerEvents.Movement_OnGroundChecking -= Movement_OnGroundChecking;
            _playerEvents.Movement_IsClimbing -= Movement_IsClimbing;
            _playerEvents.Ability_UseAbility -= Ability_UseAbility;
            _playerEvents.Ability_Success -= Ability_Success;
        }

        private void ResetHandler()
        {
            _activateHigherJump = false;
            _isOnGround = false;
            _isMovingHorizontal = false;
            _isInWater = false;
            _inputDirection = default;
            _rigidBodyDirection = default;
        }

        private void OnMoveInput(Vector2 vector)
        {
            _inputDirection = vector;
        }

        private void Movement_VelocityTransmit(Vector2 obj)
        {
            _rigidBodyDirection = obj.normalized;

            float value = obj.normalized.magnitude;

            //To-Do: Tweak values for better control;
            _playerEvents.Animator_MovementSpeed(value);

            //To-Do: Adapt that to Character Rotation

            float valueToCheck = 0.1f;

            _playerEvents.Animator_IsPulling?.Invoke(_player.lookDirection > 0 ? obj.x < -1 * valueToCheck : obj.x > valueToCheck);
            _playerEvents.Animator_IsPushing?.Invoke(_player.lookDirection > 0 ? obj.x > valueToCheck : obj.x < -1 * valueToCheck);
        }

        private void Movement_IsClimbing(bool obj)
        {
            bool value = obj && _playerStateType == PlayerStateType.Climbing;

            isCli = value;

            switch (_characterType)
            {
                case CharacterType.Stone:
                    _playerEvents.Sound_LoopSound?.Invoke(PlayerSoundType.Tamo_Climb, value);
                    break;
                case CharacterType.Plant:
                    _playerEvents.Sound_LoopSound?.Invoke(PlayerSoundType.Cenali_Climb, value);
                    break;
            }


            //if both animators have been filled please stop the animation if the input is 0
            if (_animator != null && _animatorClimb != null)
            {
                if (value == false)
                {
                    _animator.speed = 0f;
                    _animatorClimb.speed = 0f;
                }
                else
                {
                    _animator.speed = 1f;
                    _animatorClimb.speed = 1f;
                }
            }
        }

        private void Movement_OnGroundChecking(bool isOnGround, float airTime)
        {
            if (_isOnGround != isOnGround && isOnGround)
            {
                //Play Landing VFX here
                _projectEvents.VFX_Landing?.Invoke(_landingVFXTransform.position, airTime);
            }

            _isOnGround = isOnGround;

            _playerEvents.Animator_IsGrounded?.Invoke(_isOnGround);

            CheckMovement();
        }

        private void Movement_OnMovingHorizontal(bool obj)
        {
            _isMovingHorizontal = obj;

            CheckMovement();
        }

        private void CheckMovement()
        {
            bool active = _isOnGround 
                && (
                _playerStateType == PlayerStateType.Idle
                || _playerStateType == PlayerStateType.Grabbing);


            _playerEvents.Sound_LoopSound?.Invoke(
                _isInWater
                ?
                (
                    _characterType == CharacterType.Stone
                    ? PlayerSoundType.Tamo_WaterWalk
                    : PlayerSoundType.Cenali_WaterWalk
                )
                : PlayerSoundType.Player_Steps,
                 active && (Mathf.Abs(_rigidBodyDirection.x) > 0)
                );
                
        }

        private void Ability_UseAbility(bool isOn)
        {
            //To-Do: Make Fire SFX and Animator loopable

            if (!isOn)
                return;


            int direction = DetectDirectionHelper.Get4DirectionVector(_inputDirection.normalized).y;

            _playerEvents.Animator_AbilityDirection?.Invoke(direction);

            _playerEvents.Animator_IsUsingAbility?.Invoke();

            switch (_characterType)
            {
                case CharacterType.Fire:
                    _playerEvents.Sounds_SingleSound(PlayerSoundType.Mukao_Ability);
                    break;
                case CharacterType.Stone:
                    _playerEvents.Sounds_SingleSound(PlayerSoundType.Tamo_Ability);
                    break;
                case CharacterType.Plant:
                    _playerEvents.Sounds_SingleSound(PlayerSoundType.Cenali_Ability);
                    break;
                default:
                    break;
            }
        }

        private void Ability_Success()
        {
            switch (_characterType)
            {
                case CharacterType.Fire:
                    _playerEvents.Sounds_SingleSound(PlayerSoundType.Mukao_Ability); //To-Do Check later!!!
                    break;
                case CharacterType.Stone:
                    _playerEvents.Sounds_SingleSound(PlayerSoundType.Tamo_TurnToStone);
                    break;
                case CharacterType.Plant:
                    _playerEvents.Sounds_SingleSound(PlayerSoundType.Cenali_Ability); //To-Do Check later!!!
                    break;
                default:
                    break;
            }
        }

        private void Movement_ActivateHigherJump(bool obj)
        {
            _activateHigherJump = obj;
        }

        private void State_DistributeUpdatedPlayerState(PlayerStateType type)
        {
            if (_playerStateType == type)
                return;

            //new State
            switch (type)
            {
                case PlayerStateType.Idle:
                    if (_playerStateType == PlayerStateType.Falling || _playerStateType == PlayerStateType.Jumping)
                        _playerEvents.Sounds_SingleSound(PlayerSoundType.Character_Land);

                    break;
                case PlayerStateType.Dead:
                    //PlayerDied Sound & Animation
                    break;
                case PlayerStateType.Jumping:
                    _playerEvents.Animator_IsJumping?.Invoke();
                    _playerEvents.Sounds_SingleSound(_characterType == CharacterType.Fire && _activateHigherJump ? PlayerSoundType.Mukao_FireJump : PlayerSoundType.Character_Jump);
                    break;
                case PlayerStateType.Falling:
                    break;
                case PlayerStateType.Grabbing:
                    _playerEvents.Animator_IsGrabbing?.Invoke(true);
                    break;
                case PlayerStateType.Climbing:
                    _playerEvents.Animator_IsClimbing?.Invoke(true);
                    _playerEvents.Animator_IsGrounded?.Invoke(false);
                    break;
            }

            //old state
            switch (_playerStateType)
            {
                case PlayerStateType.Idle:
                    break;
                case PlayerStateType.Dead:
                    break;
                case PlayerStateType.Jumping:
                    break;
                case PlayerStateType.Falling:
                    break;
                case PlayerStateType.Grabbing:
                    _playerEvents.Animator_IsGrabbing?.Invoke(false);
                    break;
                case PlayerStateType.Climbing:
                    Movement_IsClimbing(false);
                    _playerEvents.Animator_IsClimbing?.Invoke(false);
                    //start the animator again if the character is not climbing anymore
                    if (_animatorClimb != null && _animator != null)
                    {
                        _animator.speed = 1f;
                        _animatorClimb.speed = 1f;
                    }
                    break;
                default:
                    break;
            }

            _playerStateType = type;
            CheckMovement();
        }

    }
}

