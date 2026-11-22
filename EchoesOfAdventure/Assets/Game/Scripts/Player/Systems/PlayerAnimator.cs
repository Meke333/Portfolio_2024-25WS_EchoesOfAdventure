
using System;
using UnityEngine;

namespace Player
{
	public class PlayerAnimator : PlayerSystem
	{
		[SerializeField] private Animator _animator;
		[SerializeField] private Animator _climbAnimator;

        private float _movementSpeed;
		private bool _isUsingAbility;
		private bool _isGrabbing;
		private bool _isPulling;
		private bool _isPushing;
		private bool _isClimbing;
		private bool _isGrounded;
        private int _abilityDirection;

        #region Unity Methods
        private void OnEnable()
        {
            _playerEvents.Animator_MovementSpeed += ChangeMovementSpeed;
            _playerEvents.Animator_IsJumping += TriggerIsJumping;
            _playerEvents.Animator_IsUsingAbility += ChangeIsUsingAbility;
            _playerEvents.Animator_IsGrabbing += ChangeIsGrabbing;
            _playerEvents.Animator_IsPulling += ChangeIsPulling;
            _playerEvents.Animator_IsPushing += ChangeIsPushing;
            _playerEvents.Animator_IsClimbing += ChangeIsClimbing;
            _playerEvents.Animator_IsGrounded += ChangeIsGrounded;
            _playerEvents.Animator_AbilityDirection += ChangeAbilityDirection;
        }


        private void OnDisable()
        {
            _playerEvents.Animator_MovementSpeed -= ChangeMovementSpeed;
            _playerEvents.Animator_IsJumping -= TriggerIsJumping;
            _playerEvents.Animator_IsUsingAbility -= ChangeIsUsingAbility;
            _playerEvents.Animator_IsGrabbing -= ChangeIsGrabbing;
            _playerEvents.Animator_IsPulling -= ChangeIsPulling;
            _playerEvents.Animator_IsPushing -= ChangeIsPushing;
            _playerEvents.Animator_IsClimbing -= ChangeIsClimbing;
            _playerEvents.Animator_IsGrounded -= ChangeIsGrounded;
            _playerEvents.Animator_AbilityDirection -= ChangeAbilityDirection;
        }

        #endregion

        private void ChangeMovementSpeed(float value)
        {
            _movementSpeed = value;

            _animator.SetFloat(Constants.AnimatorParameters.MovementSpeed, _isGrounded ? value : 0);
        }

        private void TriggerIsJumping()
        {
            _animator.SetTrigger(Constants.AnimatorParameters.IsJumping);
        }

        private void ChangeIsUsingAbility()
        {
            _animator.SetTrigger(Constants.AnimatorParameters.IsUsingAbility);
        }

        private void ChangeIsGrabbing(bool obj)
        {
            _isGrabbing = obj;
            _animator.SetBool(Constants.AnimatorParameters.IsGrabbing, obj);
        }

        private void ChangeIsPulling(bool obj)
        {
            _isPulling = obj;
            _animator.SetBool(Constants.AnimatorParameters.IsPulling, obj);
        }

        private void ChangeIsPushing(bool obj)
        {
            _isPushing = obj;
            _animator.SetBool(Constants.AnimatorParameters.IsPushing, obj);
        }

        private void ChangeIsClimbing(bool obj)
        {
            _isClimbing = obj;

            _animator.SetBool(Constants.AnimatorParameters.IsClimbing, obj); 
            
            if (_climbAnimator != null)
                _climbAnimator?.SetBool(Constants.AnimatorParameters.IsClimbing, obj);
        }

        private void ChangeIsGrounded(bool obj)
        {
            _isGrounded = obj;
            _animator.SetBool(Constants.AnimatorParameters.IsGrounded, obj);

            if (!_isGrounded)
                _animator.SetFloat(Constants.AnimatorParameters.MovementSpeed, 0);
        }

        private void ChangeAbilityDirection(int obj)
        {
            _abilityDirection = obj;

            Debug.Log("HAHAHHA" + obj);

            switch (_abilityDirection)
            {
                case -1:
                    //Look Down
                    _animator.SetInteger(Constants.AnimatorParameters.AbilityDirection, -1);
                    break;
                case 0:
                    //Neutral
                    _animator.SetInteger(Constants.AnimatorParameters.AbilityDirection, 0);
                    break;
                case 1:
                    //Look Up
                    _animator.SetInteger(Constants.AnimatorParameters.AbilityDirection, 1);
                    break;
                default:
                    throw new InvalidOperationException("Ability Direction is not in Range(-1, 1)");
            }
        }
    }
}