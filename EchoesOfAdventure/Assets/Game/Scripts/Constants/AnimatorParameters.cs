using UnityEngine;

namespace Constants
{
    public static class AnimatorParameters
    {
        public static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
        public static readonly int IsJumping = Animator.StringToHash("isJumping");
        public static readonly int IsUsingAbility = Animator.StringToHash("isUsingAbility");
        public static readonly int IsPulling = Animator.StringToHash("isPulling");
        public static readonly int IsPushing = Animator.StringToHash("isPushing");
        public static readonly int IsClimbing = Animator.StringToHash("isClimbing");
        public static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        public static readonly int IsGrabbing = Animator.StringToHash("isGrabbing");
        public static readonly int AbilityDirection = Animator.StringToHash("AbilityDirection");

        public static readonly int InGameUI_CollectibleCollected = Animator.StringToHash("Collectible");
        public static readonly int InGameUI_IsShowing = Animator.StringToHash("IsShowing");

        public static readonly int CharacterRunSelectionUI_IsOnRunSelection = Animator.StringToHash("IsOnRunSelection");
    }
}
