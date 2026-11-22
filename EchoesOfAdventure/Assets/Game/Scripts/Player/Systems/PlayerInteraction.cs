using Helper;
using Interactable;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Player
{
    /// <summary>
    /// Triggers and tracks possible interactions between the Player and an Object
    /// </summary>
    public class PlayerInteraction : PlayerSystem
    {
        [SerializeField] private Transform _hitbox;
        [SerializeField] private LayerMask _layerMask 
            = LayerAndMaskHelper.ConvertNumberIntoLayer(Constants.Layers.Interactables) 
            + LayerAndMaskHelper.ConvertNumberIntoLayer(Constants.Layers.Echoes);

        [SerializeField] private Rigidbody2D _playerRigidbody;

        private IInteractable _nearestInteractable;


        private Action onCancel;

        private bool _checkIsActive;

        private PlayerStateType _playerState;


        #region Unity Methods
        private void OnEnable()
        {
            onCancel += OnCancel;

            _playerEvents.State_DistributeUpdatedPlayerState += DistributeUpdatedPlayerState;
            _playerEvents.Input_OnInteractInput += InteractInput;
            _playerEvents.Input_OnJumpInput += JumpInput;
            _playerEvents.Input_OnMoveInput += DirectionInput;
        }

        private void OnDisable()
        {
            onCancel -= OnCancel;

            _playerEvents.Input_OnInteractInput -= InteractInput;
            _playerEvents.Input_OnJumpInput -= JumpInput;
            _playerEvents.Input_OnMoveInput -= DirectionInput;
        }

        #endregion


        private void DistributeUpdatedPlayerState(PlayerStateType type)
        {
            if (_playerState == type)
                return;

            switch (_playerState)
            {
                case PlayerStateType.Grabbing:
                    _nearestInteractable.StopInteracting();
                    _nearestInteractable = null;
                    break;
                case PlayerStateType.Climbing:
                    _nearestInteractable.StopInteracting();
                    _playerEvents.State_RequestingClimbState?.Invoke(false);
                    _nearestInteractable = null;

                    break;
            }

            _playerState = type;
        }


        private void JumpInput(bool obj)
        {
            if (!obj)
                return;

            switch (_player.PlayerState)
            {
                case PlayerStateType.Dead:
                case PlayerStateType.Jumping:
                case PlayerStateType.Falling:
                case PlayerStateType.Climbing:
                    _playerEvents.State_RequestingClimbState?.Invoke(false);
                    return;
                case PlayerStateType.Grabbing:
                    _playerEvents.State_RequestingGrabState?.Invoke(false);
                    return;
            }
        }


        private void InteractInput(bool obj)
        {
            if (!obj)
                return;

            switch (_player.PlayerState)
            {
                case PlayerStateType.Dead:
                case PlayerStateType.Jumping:
                case PlayerStateType.Falling:
                case PlayerStateType.Climbing:
                    return;
                case PlayerStateType.Grabbing:
                    _playerEvents.State_RequestingGrabState?.Invoke(false);
                    return;
            }

            //TO-DO: Implement Grabbing Mechanic
            IGrabable grabable;

            //Collision Detection
            Collider2D[] collider2Ds = Collider2DHelper.GetHitArray<Collider2D>(_hitbox, _layerMask, true).OrderBy(c => (_hitbox.transform.position - c.transform.position).sqrMagnitude).ToArray();
        
            for(int i = 0; i < collider2Ds.Length; i++)
            {
                IInteractable temp = collider2Ds[i].gameObject.GetComponentInParent<IInteractable>();

                if (temp == null)
                    continue;

                if (!temp.IsInteractionAllowed(_player.CharacterType))
                    continue;

                grabable = collider2Ds[i].gameObject.GetComponentInParent<IGrabable>();

                if (grabable == null)
                    continue;
                _nearestInteractable = temp;
                grabable.TransmitRigidbody(_playerRigidbody);
                grabable.NotifyOnRCancel(this);

                break;
            }
            if (_nearestInteractable == null)
                return;

            
            _nearestInteractable.StartInteracting();
            _playerEvents.State_RequestingGrabState?.Invoke(true);

        }


        //TO-DO: Only do Climbable Detection and teleport player on the x-Axis to the origin spot
        private void DirectionInput(Vector2 vector)
        {
            if (_playerState != PlayerStateType.Idle)
                return;

            //Climbing behaviour

            Vector2Int input = DetectDirectionHelper.Get4DirectionVector(vector);
            int x = Mathf.Abs(input.x);
            int y = Mathf.Abs(input.y);

            if (x <= 0 && y <= 0)
                return;

            switch (_player.PlayerState)
            {
                case PlayerStateType.Climbing:
                case PlayerStateType.Dead:
                case PlayerStateType.Grabbing:
                    return;
            }

            if (x > y)
                return;

            IClimbable climbable;

            //Collision Detection
            Collider2D[] collider2Ds = Collider2DHelper.GetHitArray<Collider2D>(_hitbox, _layerMask, true).OrderBy(c => (_hitbox.transform.position - c.transform.position).sqrMagnitude).ToArray();

            for (int i = 0; i < collider2Ds.Length; i++)
            {
                IInteractable temp = collider2Ds[i].gameObject.GetComponentInParent<IInteractable>();

                if (temp == null)
                    continue;

                if (!temp.IsInteractionAllowed(_player.CharacterType))
                    continue;

                climbable = collider2Ds[i].gameObject.GetComponentInParent<IClimbable>();

                if (climbable == null)
                    continue;

                _nearestInteractable = temp;
                //onCancel = climbable.NotifyOnReachedTop();
                _playerEvents.Movement_SnapToXLocation?.Invoke(collider2Ds[i].transform.position.x);
                break;
            }

            if (_nearestInteractable == null)
                return;

            _nearestInteractable.StartInteracting();
            _playerEvents.State_RequestingClimbState?.Invoke(true);
        }

        private void EvaluateHitbox()
        {
            Collider2D[] collider2Ds = Collider2DHelper.GetHitArray<Collider2D>(_hitbox, _layerMask).OrderBy(c => (_hitbox.transform.position - c.transform.position).sqrMagnitude).ToArray();
          
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                InteractableBase temp = collider2Ds[i].gameObject.GetComponent<InteractableBase>();

                if (temp == null)
                    continue;

                if (!temp.IsInteractionAllowed(_player.CharacterType))
                    continue;

                _nearestInteractable = temp;
                break;
            }

        }

        public void OnCancel()
        {
            switch (_player.PlayerState)
            {
                case PlayerStateType.Idle:
                case PlayerStateType.Dead:
                case PlayerStateType.Jumping:
                case PlayerStateType.Falling:
                    break;
                case PlayerStateType.Grabbing:
                    _playerEvents.State_RequestingGrabState(false);
                    break;
                case PlayerStateType.Climbing:
                    break;
                default:
                    break;
            }
        }
    }
}

