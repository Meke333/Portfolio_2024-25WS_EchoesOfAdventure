using Configuration;
using Echo;
using Helper;
using Interface;
using Player;
using System;
using Events;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Interactable
{
    [SelectionBase]
    public class WoodBox : InteractableBase, IBurnable, IStoneable, IStompable, IGrabable
    {
        [Inject]private ProjectEvents _projectEvents;
        
        [Inject] private GeneralPrefabConfigurations _prefabConfigurations;
        [Inject] private GeneralVisualConfigurations _visualConfigurations;
        [Inject] private InteractableConfigurations _interactableConfigurations;

        [SerializeField] private Rigidbody2D _rigidBody;
        [SerializeField] private Joint2D _joint;
        [SerializeField] private float _grabbedMass;
        [SerializeField] private Transform _colliderTransform;
        [SerializeField] private float _groundCheckDistance;
        [SerializeField] private LayerMask _groundMask;

        [SerializeField] private InteractableType _interactableType;

        [SerializeField] private Transform _dustVFXPosition;
        [SerializeField] private float _dustMultiplier = 7f;

        [Header("Graphics")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private GameObject _stoneEchoVFXGameObject;

        [Header("Sounds")]
        [SerializeField] private GameObject _soundEmitterGameObject;

        private bool _isInteractedWith;
        private Action OnCancel;
        private float _setMass;
        private bool _isGrounded;
        private bool _isGrabbed;

        private PlayerInteraction playerInteraction;


        private bool isGrounded
        { 
            get => _isGrounded;
            set 
            { 
                if (State == InteractableState.Normal && _isInteractedWith && !value)
                {
                    playerInteraction.OnCancel();
                    _isInteractedWith = false;

                    _interactableEvents.WoodenBox_Drag?.Invoke(false);
                }

                _isGrounded = value;
            }
        }

        private float _time;
        private float time
        {
            get => _time;
            set
            {
                if (value < 0 || !_isStepped)
                {
                    return;
                }
                else if (value >= _interactableConfigurations.TimeBeforeWoodboxBreaks)
                {
                    BreakStomp();
                    return;
                }
                _time = value;
            }
        }

        private bool _isStepped;

        private void Awake()
        {
            if (State == 0)
                State = InteractableState.Normal;

            _setMass = _rigidBody.mass;
        }

        private void Update()
        {
            time += _isStepped ? Time.deltaTime : -Time.deltaTime;
        }

        private void FixedUpdate()
        {
            CheckGroundCollision();
            CheckPushingRigidbody();
        }


#if UNITY_EDITOR

        private void OnValidate()
        {
            _setMass = _rigidBody.mass;
        }

#endif

        public void DestroyBox()
        {
            _interactableEvents.WoodenBox_Breaks?.Invoke(transform.position);
            SetActivity(false);
        }

        public void DestroyBoxQuiet()
        {
            SetActivity(false);
        }

        private void CheckGroundCollision()
        {
            bool _previousGrounded = isGrounded;

            isGrounded = Physics2D.BoxCast(
                _colliderTransform.position,
                _colliderTransform.localScale / 2,
                0,
                Vector2.down,
                _groundCheckDistance,
                _groundMask);

            if (_previousGrounded != isGrounded && isGrounded)
            {
                _interactableEvents.WoodenBox_Lands?.Invoke(transform.position);
                _projectEvents.VFX_Landing?.Invoke(_dustVFXPosition.position, Mathf.Abs(_rigidBody.linearVelocity.y) * _dustMultiplier);
            }
        }


        private void CheckPushingRigidbody()
        {
            if (!_isInteractedWith)
                return;

            _interactableEvents.WoodenBox_Drag?.Invoke(Mathf.Abs(_rigidBody.linearVelocity.x) > 0.01f);
        }


        #region InteractableBase
        protected override void NormalBehaviourState()
        {
            return;
        }

        protected override void DestroyedBehaviourState()
        {
            DestroyBoxQuiet();
        }

        protected override void BurnedBehaviourState()
        {
            Debug.Log("Burning");
            BurnDown();
        }

        protected override void SproutedBehaviourState()
        {
            return;
        }

        protected override void StoneBehaviourState()
        {
            Debug.Log("Stoning");
            TurnToStone();
        }
        #endregion

        #region IPoolObject

        public override void ResetObject()
        {
            Debug.Log("ResetObject Woodbox");
            //State = InteractableState.Normal;
            _rigidBody.bodyType = RigidbodyType2D.Dynamic;
            _isInteractedWith = false;
            _isStepped = false;
            time = 0;
            _renderer.material = _visualConfigurations.NormalWoodboxMaterial;
            _stoneEchoVFXGameObject.SetActive(false);

            _interactableEvents.WoodenBox_Drag?.Invoke(false);
            _soundEmitterGameObject.SetActive(false);
            //_interactableEvents.Fire_Burning?.Invoke(transform.position, false);
        }
        #endregion

        #region IInteractable
        protected override void SetInteractableType()
        {
            interactableType = InteractableType.Woodbox;
        }

        public override void StartInteracting()
        {
            _isInteractedWith = true;
            _joint.enabled = true;
            _rigidBody.mass = _grabbedMass;
            Vector3 newTransform = new Vector3
                (
                transform.position.x + _interactableConfigurations.WoodboxResetPositionWhenInteracted.x
                , transform.position.y + _interactableConfigurations.WoodboxResetPositionWhenInteracted.y
                , transform.position.z
                );

            transform.position = newTransform;
        }

        public override void StopInteracting()
        {
            _isInteractedWith = false;
            _joint.enabled = false;
            _joint.connectedBody = null;
            _rigidBody.mass = _setMass;

            playerInteraction = null;

            _interactableEvents.WoodenBox_Drag?.Invoke(false);
        }

        public override bool IsInteractionAllowed(CharacterType type)
        {
            if (State != InteractableState.Normal)
                return false;

            return LayerAndMaskHelper.IsTypeInMask(characterMask, type);
        }

        #endregion

        #region IBurnable
        public bool CanBurnDown()
        {

            //if (!((State & (InteractableState.Normal | InteractableState.Sprouted)) != 0)) Only works if Enum is flagged!!
            switch (State)
            {
                case InteractableState.Normal:
                case InteractableState.Sprouted:
                    return true;
            }

            return false;
        }
        public void BurnDown()
        {
            State = InteractableState.Burned;

            //RayCast
            Vector3 fireEchoPosition = new Vector3();

            float distance = 100f;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, distance, LayerAndMaskHelper.ConvertNumberIntoLayer(Constants.Layers.Ground));

            if (raycastHit2D)
            {
                fireEchoPosition = raycastHit2D.point;
                poolManager.ActivateInOtherManager(typeof(FireEchoSpawner).GetHashCode(), fireEchoPosition);
            }

            SetActivity(false);

            //_interactableEvents.Fire_Burning?.Invoke(transform.position, true);


            if (playerInteraction != null)
                playerInteraction.OnCancel();
        }
        #endregion

        #region IStoneable

        public void TurnToStone()
        {
            if (!_isGrounded)
                return;

            State = InteractableState.Stone;

            //Change Material
            _renderer.material = _visualConfigurations.StoneWoodboxMaterial;

            //Change RigidBody2D Component Values
            _rigidBody.bodyType = RigidbodyType2D.Kinematic;
            _rigidBody.linearVelocity = new Vector2(0,0);
            

            //Activate VFX
            _stoneEchoVFXGameObject.SetActive(true);

            _projectEvents.TutorialBoxTurnToStone?.Invoke();
            _soundEmitterGameObject.SetActive(true);
        }

        #endregion

        #region IStompable
        public void StartStomp()
        {
            _isStepped = true;
            BreakStomp();
        }

        public void StopStomp()
        {
            _isStepped = false;
        }

        public void BreakStomp()
        {
            if (State != InteractableState.Normal)
                return;

            State = InteractableState.Destroyed;
            DestroyBox();
        }

        #endregion

        #region IGrabable


        public void NotifyOnRCancel(PlayerInteraction interaction)
        {
            playerInteraction = interaction;
        }

        public void TransmitRigidbody(Rigidbody2D body)
        {
            _joint.connectedBody = body;
        }

        #endregion
    }
}

