using System;
using UnityEngine;

namespace Player
{
    public class PlayerEvents
    {
        #region General

        public Action General_ResetPlayer;

        #endregion

        #region Input
        public Action<Vector2> Input_OnMoveInput; //Reset when Reset
        public Action<bool> Input_OnInteractInput; //Reset when Reset
        public Action<bool> Input_OnJumpInput; //Reset when Reset
        public Action<bool> Input_OnUseAbilityInput; //Reset when Reset

        #endregion

        #region Interaction & Ability

        public Action<bool> Ability_UseAbility;
        public Action Ability_Success;

        #endregion

        #region Movement
        public Action<bool> Movement_OnMovingHorizontal;
        public Action<bool, float> Movement_OnGroundChecking;  //Reset when Reset
        public Action<float> Movement_VelocityTransmitX;
        public Action<float> Movement_VelocityTransmitY;
        public Action<Vector2> Movement_VelocityTransmit;
        public Action<bool> Movement_IsClimbing;
        public Action Movement_OnJump; //Reset when Reset
        public Action<Vector2> Movement_SnapToLocation;
        public Action<float> Movement_SnapToXLocation;
        public Action<float> Movement_SnapToYLocation;
        public Action<bool> Movement_ActivateHigherJump; //Reset when Reset

        #endregion

        #region LookDirection
        public Action<int> LookDirection_OnLookingDirectionChange;  //Reset when Reset

        #endregion

        #region States

        //Reset State to Idle when Reset
        public Action<PlayerStateType> State_DistributeUpdatedPlayerState;
        public Action<bool> State_RequestingClimbState;
        public Action<bool> State_RequestingGrabState;
        public Action State_PlayerDeath;

        #endregion

        #region Sounds
        public Action<PlayerSoundType> Sounds_SingleSound;
        public Action<PlayerSoundType, bool> Sound_LoopSound;

        #endregion

        #region Animator
        public Action<float> Animator_MovementSpeed; //DONE
        public Action Animator_IsJumping; //DONE
        public Action Animator_IsUsingAbility; //DONE
        public Action<bool> Animator_IsGrabbing; //DONE
        public Action<bool> Animator_IsPulling;
        public Action<bool> Animator_IsPushing;
        public Action<bool> Animator_IsClimbing; //DONE
        public Action<bool> Animator_IsGrounded; //DONE
        public Action<int> Animator_AbilityDirection;

        #endregion

        #region External

        #endregion
    }
}

