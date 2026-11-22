
using System;
using UnityEngine;

namespace Player
{
    public class PlayerRotator : PlayerSystem
	{
		[SerializeField] private Transform _graphics;

        private readonly Quaternion left = Quaternion.Euler(0,180,0);
        private readonly Quaternion right = Quaternion.Euler(0, 0, 0);
        private readonly Quaternion backwards = Quaternion.Euler(0, -90, 0);

        private int _inputDirection;
        private int _lookDirection;
        private bool _isGrabbing;
        private bool _isClimbing;

        private PlayerStateType _stateType;

        #region Unity Methods
        private void OnEnable()
        {
            _playerEvents.LookDirection_OnLookingDirectionChange += LookDirectionChange;
            _playerEvents.State_DistributeUpdatedPlayerState += UpdatePlayerState;
            
        }

        private void OnDisable()
        {
            _playerEvents.LookDirection_OnLookingDirectionChange -= LookDirectionChange;
            _playerEvents.State_DistributeUpdatedPlayerState -= UpdatePlayerState;
           
        }

        #endregion

        private void LookDirectionChange(int direction)
        {
            _inputDirection = direction;

            switch (_player.PlayerState)
            {
                case PlayerStateType.Grabbing:
                case PlayerStateType.Climbing:
                    return;
            }

            SetHorizontalDirection();
        }

        private void UpdatePlayerState(PlayerStateType type)
        {


            switch (type)
            {
                case PlayerStateType.Grabbing:
                    _isGrabbing = true;
                    _isClimbing = false;
                    break;
                case PlayerStateType.Idle:
                case PlayerStateType.Dead:
                case PlayerStateType.Jumping:
                case PlayerStateType.Falling:
                    _isGrabbing = false;
                    _isClimbing = false;
                    SetHorizontalDirection();
                    break;
                case PlayerStateType.Climbing:
                    _isGrabbing = false;
                    _isClimbing = true;
                    SetDepthDirection();
                    break;
            }

            _stateType = type;
        }


        private void SetDepthDirection()
        {
            _graphics.rotation = backwards;
        }


        private void SetHorizontalDirection()
        {
            if (_isGrabbing == true || _isClimbing == true)
                return;

            _lookDirection = _inputDirection;

            //old type was grabbing
            if (_stateType == PlayerStateType.Grabbing)
                return;

            if (_lookDirection < 0)
            {
                _graphics.rotation = left;
            }
            else if (_lookDirection > 0)
            {
                _graphics.rotation = right;
            }

        }

        private void OnMovingHorizontal(float obj)
        {

            if (Mathf.Abs(obj) > 0.1f)
                return;

            if (_stateType != PlayerStateType.Idle)
                return;

            SetHorizontalDirection();
        }
    }
}