using NaughtyAttributes;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = nameof(PlayerMovementConfiguration), menuName = Constants.Project.AssetMenuStart + nameof(PlayerMovementConfiguration))]
    public class PlayerMovementConfiguration : ScriptableObject
    {
        //Movement
        [field: SerializeField]
        [field: Tooltip("The top horizontal movement speed")]
        public float MaxSpeed { get; private set; } = 14;

        [field: SerializeField]
        [field: Tooltip("The player's capacity to gain horizontal speed")]
        public float Acceleration { get; private set; } = 120;

        [field: SerializeField]
        [field: Tooltip("The pace at which the player comes to a stop")]
        public float GroundDeceleration { get; private set; } = 60;

        [field: SerializeField]
        [field: Tooltip("Deceleration in air only after stopping input mid-air")]
        public float AirDeceleration { get; private set; } = 30;

        [field: SerializeField]
        [field: Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
        public float GroundingForce { get; private set; } = -1.5f;

        [field: SerializeField]
        [field: Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
        public float GrounderDistance { get; private set; } = 0.05f;

        [field: SerializeField]
        [Header("JUMP")]
        [field: Tooltip("The immediate velocity applied when jumping")]
        public float JumpPower { get; private set; } = 36;

        [field: SerializeField]
        [field: Tooltip("The special and immediate velocity applied when jumping with specific conditions met")]
        public float SpecialJumpPower { get; private set; } = 60;

        [field: SerializeField]
        [field: Tooltip("The maximum vertical movement speed")]
        public float MaxFallSpeed { get; private set; } = 40;

        [field: SerializeField]
        [field: Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
        public float FallAcceleration { get; private set; } = 110;

        [field: SerializeField]
        [field: Tooltip("The gravity multiplier added when jump is released early")]
        public float JumpEndEarlyGravityModifier { get; private set; } = 3;

        [field: SerializeField]
        [field: Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
        public float CoyoteTime { get; private set; } = .15f;

        [field: SerializeField]
        [field: Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
        public float JumpBuffer { get; private set; } = .2f;

        [field: SerializeField]
        [field: Tooltip("On which layer can the player jump from?")]
        public LayerMask JumpLayer { get; private set; } = (LayerMask) ~Constants.Layers.Player;
        [field: SerializeField]
        [field: Tooltip("On which layer does the player hit the ceiling?")] 
        public LayerMask CeilingLayer { get; private set; } = (LayerMask) ~Constants.Layers.Player;


    }

    

}
