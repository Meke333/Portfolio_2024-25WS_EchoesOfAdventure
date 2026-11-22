using Helper;
using NaughtyAttributes;
using System;
using UnityEngine;
using Zenject;


namespace Player
{
    /// <summary>
    /// Manages Player Movement, like moving, jumping and falling
    /// </summary>
    public class PlayerMovement : PlayerSystem
    {
        [Inject] private DiContainer container;

        [Inject] private PlayerMovementConfiguration _movementConfiguration;
        [Inject] private PlayerConfiguration _playerConfiguration;

        [SerializeField] private float _climbUpAtStart = 0.1f;

        [SerializeField] private Rigidbody2D _rigidbody;
        //[SerializeField] private Transform _colliderParent;

        //private Collider[] _colliders;
        [SerializeField] private CapsuleCollider2D _capsuleCollider2D;
        [SerializeField] private BoxCollider2D _boxCollider2D;

        private Vector2 _velocity;

        private MoveInputData _moveInputData;

        private float _time;
        private bool _cachedQueryStartInColliders;
        private float _normalGravityScale;
        private RigidbodyConstraints2D _normalRigidbodyConstraints;

        private bool _isHigherJumpActive;


        [SerializeField] private MovementType _movementType = MovementType.Normal;

        #region Debug

#if DEBUG || UNITY_EDITOR
        [SerializeField, ReadOnly, Foldout("Debug: Velocity")] private Vector2 _currentVelocity;
        [SerializeField, ReadOnly, Foldout("Debug: MoveInput")] private Vector2 _moveDirection;
#endif

        #endregion



        #region Unity Methods

        private void Awake()
        {
            //_colliders = _colliderParent.GetComponentsInChildren<Collider>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
            _normalGravityScale = _rigidbody.gravityScale;
            _normalRigidbodyConstraints = _rigidbody.constraints;
        }

        private void OnEnable()
        {
            _playerEvents.State_DistributeUpdatedPlayerState += DistributeUpdatedPlayerState;
            _playerEvents.Input_OnMoveInput += MoveInput;
            _playerEvents.Input_OnJumpInput += JumpInput;
            _playerEvents.Movement_SnapToLocation += SnapToLocation;
            _playerEvents.Movement_SnapToXLocation += SnapToXLocation;
            _playerEvents.Movement_SnapToYLocation += SnapToYLocation;
            _playerEvents.Movement_ActivateHigherJump += ActivateHigherJump;

            _moveInputData = new MoveInputData();
        }

        private void OnDisable()
        {
            _playerEvents.State_DistributeUpdatedPlayerState -= DistributeUpdatedPlayerState;
            _playerEvents.Input_OnMoveInput -= MoveInput;
            _playerEvents.Input_OnJumpInput -= JumpInput;
            _playerEvents.Movement_SnapToLocation -= SnapToLocation;
            _playerEvents.Movement_SnapToXLocation -= SnapToXLocation;
            _playerEvents.Movement_SnapToYLocation -= SnapToYLocation;
            _playerEvents.Movement_ActivateHigherJump -= ActivateHigherJump;
        }


        private void Update()
        {
            _time += Time.deltaTime;

        }

        private void FixedUpdate()
        {
            switch (_movementType)
            {
                case MovementType.None:
                    return;
                case MovementType.Normal:
                    JumpAndRun_HandleGravity();
                    JumpAndRun_CheckCollisions();
                    JumpAndRun_HandleJump();
                    JumpAndRun_HandleDirection();
                    break;

                case MovementType.Climbing:
                    Climbing_CheckCollision();
                    Climbing_HandleDirection();
                    Climbing_HandleJump();
                    Climbing_HandleGravity();
                    break;
                case MovementType.Grabbing:
                    JumpAndRun_CheckCollisions();
                    JumpAndRun_HandleDirection();
                    break;
                default:
                    break;
            }

            ApplyMovement();

#if DEBUG || UNITY_EDITOR
            _currentVelocity = _rigidbody.linearVelocity;

#endif

        }

        #endregion

        #region Events

        private void DistributeUpdatedPlayerState(PlayerStateType type)
        {
            switch (type)
            {
                case PlayerStateType.Dead:
                    _movementType = MovementType.None;
                    break;
                case PlayerStateType.Idle:
                case PlayerStateType.Jumping:
                case PlayerStateType.Falling:
                    _movementType = MovementType.Normal;
                    //_rigidbody.constraints = _normalRigidbodyConstraints;
                    _rigidbody.gravityScale = _normalGravityScale;
                    break;
                case PlayerStateType.Grabbing:
                    _movementType = MovementType.Grabbing;
                    break;
                case PlayerStateType.Climbing:
                    _movementType = MovementType.Climbing;
                    //_rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
                    //_rigidbody.freezeRotation = true;
                    _velocity = Vector2.zero;
                    _rigidbody.position += Vector2.up * _climbUpAtStart;
                    _rigidbody.gravityScale = 0f;
                    break;
            }
        }

        private void MoveInput(Vector2 vector)
        {
            _moveInputData.moveDirection = vector;

#if DEBUG || UNITY_EDITOR
            _moveDirection = vector;
#endif
        }

        private void JumpInput(bool isJumpButtonDown)
        {
            //Debug.Log(isJumpButtonDown);
            _moveInputData.isJumpButtonDown = isJumpButtonDown;

            if (_moveInputData.isJumpButtonDown)
            {
 //Call Method!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                _AnyJumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
        }
#endregion

        #region Jump & Run
        #region Collision
        private float _frameLeftGrounded = float.MinValue;
        private bool _isGrounded; 

        private void JumpAndRun_CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            //Checking Ground & Ceiling
            bool groundHit = 
                Physics2D.BoxCast
                (
                _boxCollider2D.bounds.center,
                _boxCollider2D.size,
                0,
                Vector2.down,
                _movementConfiguration.GrounderDistance, 
                _movementConfiguration.JumpLayer
                ) 
                || Physics2D.CapsuleCast
                (
                _capsuleCollider2D.bounds.center,
                _capsuleCollider2D.size,
                _capsuleCollider2D.direction,
                0,
                Vector2.down,
                _movementConfiguration.GrounderDistance,
                _movementConfiguration.JumpLayer
                );

            bool ceilingHit = Physics2D.CapsuleCast(
                _capsuleCollider2D.bounds.center,
                _capsuleCollider2D.size,
                _capsuleCollider2D.direction,
                0,
                Vector2.up,
                _movementConfiguration.GrounderDistance,
                _movementConfiguration.CeilingLayer
                );

            //Ceiling hit
            if (ceilingHit) 
                _velocity.y = Mathf.Min(0, _velocity.y);

            //Ground hit
            if (!_isGrounded && groundHit)
            {
                _isGrounded = true;
                _isCoyoteUseable = true;
                _isBufferedJumpUseable = true;
                _isJumpEndedEarly = false;
                _playerEvents.Movement_OnGroundChecking?.Invoke(true, Mathf.Abs(_velocity.y));
            }
            //Ground left
            else if (_isGrounded && !groundHit)
            {
                _isGrounded = false;
                _frameLeftGrounded = _time;
                _playerEvents.Movement_OnGroundChecking?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion

        #region Jump

        private bool _AnyJumpToConsume;
        private bool _isBufferedJumpUseable;
        private bool _isJumpEndedEarly;
        private bool _isCoyoteUseable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _isBufferedJumpUseable 
            && _time < _timeJumpWasPressed + _movementConfiguration.JumpBuffer;
        private bool CanUseCoyote => _isCoyoteUseable && !_isGrounded && _time < _frameLeftGrounded + _movementConfiguration.CoyoteTime;


        private void JumpAndRun_HandleJump()
        {
            if (!_isJumpEndedEarly 
                && !_isGrounded 
                && !_moveInputData.isJumpButtonDown 
                && _rigidbody.linearVelocity.y > 0)
                _isJumpEndedEarly = true;

            if (!_AnyJumpToConsume && !HasBufferedJump)
                return;

            if (_moveInputData.isJumpButtonDown && (_isGrounded || CanUseCoyote))
                ExecuteJump();

            _AnyJumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _isJumpEndedEarly = false;
            _timeJumpWasPressed = 0;
            _isBufferedJumpUseable = false;
            _isCoyoteUseable = false;
            _velocity.y = _isHigherJumpActive 
                ? _movementConfiguration.SpecialJumpPower
                : _movementConfiguration.JumpPower;
            _playerEvents.Movement_OnJump?.Invoke();
        }

        #endregion

        #region Horizontal

        private void JumpAndRun_HandleDirection()
        {
            if (_moveInputData.moveDirection.x == 0)
            {
                var deceleration = _isGrounded 
                    ? _movementConfiguration.GroundDeceleration 
                    : _movementConfiguration.AirDeceleration;

                _velocity.x = Mathf.MoveTowards(_velocity.x, 
                    0,
                    deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _velocity.x = Mathf.MoveTowards(_velocity.x,
                    _moveInputData.moveDirection.x * _movementConfiguration.MaxSpeed,
                    _movementConfiguration.Acceleration * Time.fixedDeltaTime);

            }

            _playerEvents.Movement_OnMovingHorizontal?.Invoke(Mathf.Abs(_velocity.x) > 0);
            _playerEvents.Movement_VelocityTransmitX?.Invoke(_rigidbody.linearVelocity.x);
            _playerEvents.Movement_VelocityTransmit?.Invoke(_rigidbody.linearVelocity);
        }

        #endregion

        #region Gravity

        private void JumpAndRun_HandleGravity()
        {
            if (_isGrounded && _velocity.y <= 0f)
            {
                _velocity.y = _movementConfiguration.GroundingForce;
            }
            else
            {
                float inAirGravity = _movementConfiguration.FallAcceleration;
                if (_isJumpEndedEarly && _velocity.y > 0)
                    inAirGravity *= _movementConfiguration.JumpEndEarlyGravityModifier;

                _velocity.y = Mathf.MoveTowards(_velocity.y, -_movementConfiguration.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }

            _playerEvents.Movement_VelocityTransmitY?.Invoke(_rigidbody.linearVelocity.y);
        }
        #endregion

        #endregion

        #region Climbing

        private void Climbing_CheckCollision()
        {
            Physics2D.queriesStartInColliders = false;

            //Check if there is a ground (if there is a ground stop climbing)
            bool groundHit = Physics2D.BoxCast
                (
                _boxCollider2D.bounds.center,
                _boxCollider2D.size,
                0,
                Vector2.down,
                _movementConfiguration.GrounderDistance,
                _movementConfiguration.JumpLayer
                );


            //Check if there is a ceiling (if there is a ceiling you can't move up)
            bool ceilingHit = Physics2D.CapsuleCast(
                _capsuleCollider2D.bounds.center,
                _capsuleCollider2D.size,
                _capsuleCollider2D.direction,
                0,
                Vector2.up,
                _movementConfiguration.GrounderDistance,
                LayerAndMaskHelper.ConvertNumberIntoLayer(Constants.Layers.Guide)
                );

            if (ceilingHit)
            {
                _velocity.y = Mathf.Min(0, _velocity.y);
            }

            if (groundHit)
            {
                _isGrounded = true;
                _isCoyoteUseable = true;
                _isBufferedJumpUseable = true;
                _isJumpEndedEarly = false;
                _playerEvents.Movement_OnGroundChecking?.Invoke(true, Mathf.Abs(_velocity.y));
                //End ClimbMode!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                EndClimbingMode();
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        private void Climbing_HandleJump()
        {
            if (!_moveInputData.isJumpButtonDown)
                return;

            //End ClimbMode!!!!!!!!!!!!!!!!!!!! and hope the Jump Code does the rest!
            EndClimbingMode(); 
            ExecuteJump();
        }

        private void Climbing_HandleDirection()
        {
            _velocity.x = 0;

            int input = Math.Abs(DetectDirectionHelper.Get4DirectionVector(_moveInputData.moveDirection).x);

            if (input != 0)
            {
                //End ClimbMode!!!!!!!!!!!!!!!!!!!! and hope the Jump Code does the rest!
                EndClimbingMode();
                return;
            }

            //TO-DO: Create a variable for movementConfiguration: Climbing Speed
            _velocity.y = _moveInputData.moveDirection.y * _movementConfiguration.MaxSpeed;


            _playerEvents.Movement_IsClimbing?.Invoke(Mathf.Abs(_rigidbody.linearVelocity.y) > 0.1f);
        }

        private void Climbing_HandleGravity()
        {
            return;
        }

        private void EndClimbingMode()
        {

            _playerEvents.Movement_IsClimbing?.Invoke(false);

            _playerEvents.State_RequestingClimbState?.Invoke(false);
            _movementType = MovementType.Normal;
        }

        #endregion

        private void SnapToLocation(Vector2 position) => _rigidbody.position = position;
        private void SnapToXLocation(float position) => _rigidbody.position = new Vector2(position, _rigidbody.position.y);
        private void SnapToYLocation(float position) => _rigidbody.position = new Vector2(_rigidbody.position.x, position);
        private void ActivateHigherJump(bool value) => _isHigherJumpActive = value;

        private void ApplyMovement() => _rigidbody.linearVelocity = _velocity;
    }

    public enum MovementType
    {
        None = 0,
        Normal = 1,
        Climbing = 2,
        Grabbing = 3,
    }
}

