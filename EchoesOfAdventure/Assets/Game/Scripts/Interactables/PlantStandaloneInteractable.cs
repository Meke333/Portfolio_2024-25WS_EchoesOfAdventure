using Echo;
using GameEnums;
using Events;
using Helper;
using Interactable;
using Interface;
using Player;
using System;
using UnityEngine;
using Zenject;
using NaughtyAttributes;

namespace ObjectPool
{
    [SelectionBase]
    public class PlantStandaloneInteractable : InteractableBase, IBurnable, IClimbable
    {
        [Inject] private ProjectEvents _projectEvents;
        
        [Header("Game Design")]
        [SerializeField] private DirectionType _plantDirection;
        [Tooltip("Determines size of Colliders and Visuals; Visuals only have a minimum of 0 and maximum of 6,7")]
        [SerializeField, Range(1,Constants.Values.MaximumPlantVisualHeight)] private float _plantScale = 1f;
        [SerializeField] private CharacterMask _characterPassingTroughMask;

        [Header("Collider")]
        [SerializeField] private Transform _colliderParent;
        [SerializeField] private Collider2D _normalCollider;

        [SerializeField] private Collider2D _upperGuideCollider;
        [SerializeField] private Collider2D _bottomGuideCollider;
        [SerializeField] private Collider2D _leftGuideCollider;
        [SerializeField] private Collider2D _rightGuideCollider;

        [Header("Graphics")]
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Renderer _outlineRenderer;

        [Header("Only for Testing")]
        [SerializeField] private Transform _graphicTransform;

        private Material _plantMaterial;
        private Material _outlineMaterial;
        private Collider2D[] _plantColliders;

        private Action OnCancelingClimbMode;

        private void Awake()
        {

            switch (_plantDirection)
            {
                case DirectionType.Up:
                    _upperGuideCollider.gameObject.SetActive(true);
                    _bottomGuideCollider.gameObject.SetActive(true);

                    _leftGuideCollider.gameObject.SetActive(false);
                    _rightGuideCollider.gameObject.SetActive(false);
                    break;

                case DirectionType.Down:
                    _upperGuideCollider.gameObject.SetActive(false);
                    _bottomGuideCollider.gameObject.SetActive(false);

                    _leftGuideCollider.gameObject.SetActive(false);
                    _rightGuideCollider.gameObject.SetActive(false);

                    _upperGuideCollider.enabled = true;
                    break;
                case DirectionType.Left:
                    _upperGuideCollider.gameObject.SetActive(false);
                    _bottomGuideCollider.gameObject.SetActive(false);

                    _leftGuideCollider.gameObject.SetActive(false);
                    _rightGuideCollider.gameObject.SetActive(true);

                    _rightGuideCollider.enabled = true;
                    break;
                case DirectionType.Right:
                    _upperGuideCollider.gameObject.SetActive(false);
                    _bottomGuideCollider.gameObject.SetActive(false);

                    _leftGuideCollider.gameObject.SetActive(true);
                    _rightGuideCollider.gameObject.SetActive(false);

                    _leftGuideCollider.enabled = true;
                    break;
            }

            _projectEvents.Character_CharacterSelected += CharacterSelect_CharacterSelected;
        }

        private void Start()
        {
            SetUp();
        }

        private void OnDestroy()
        {
            _projectEvents.Character_CharacterSelected -= CharacterSelect_CharacterSelected;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            //Rotation
            Quaternion rotation = Quaternion.identity;
            switch (_plantDirection)
            {
                case DirectionType.Up:
                    rotation = Quaternion.identity;
                    break;
                case DirectionType.Down:
                    rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case DirectionType.Left:
                    rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case DirectionType.Right:
                    rotation = Quaternion.Euler(0, 0, -90);
                    break;
            }
            transform.rotation = rotation;

            //Scale
            _colliderParent.localScale = new Vector3(_colliderParent.localScale.x, _plantScale, _colliderParent.localScale.z);

        }
#endif


        private void SetUp()
        {
            _plantMaterial = _renderer.material;
            _outlineMaterial = _outlineRenderer.material;
            SetGraphicValue();

            //Rotation
            Quaternion rotation = Quaternion.identity;
            switch (_plantDirection)
            {
                case DirectionType.Up:
                    rotation = Quaternion.identity;
                    break;
                case DirectionType.Down:
                    rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case DirectionType.Left:
                    rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case DirectionType.Right:
                    rotation = Quaternion.Euler(0, 0, -90);
                    break;
            }
            transform.rotation = rotation;

            //Scale
            _colliderParent.localScale = new Vector3(_colliderParent.localScale.x, _plantScale, _colliderParent.localScale.z);

            StopInteracting();
        }   
        
        private void SetGraphicValue()
        {
            float value = Mathf.Max(
                Constants.Values.MinimumPlantVisualHeight, 
                Mathf.Min(
                    _plantScale / Constants.Values.MaximumPlantVisualHeight - (Constants.Values.MinimumPlantVisualHeight * 2), 
                    Constants.Values.MaximumPlantVisualHeight));

            _plantMaterial.SetFloat("_GrowFloat", value);
            _outlineMaterial.SetFloat("_GrowFloat", value);
        }

        private void CharacterSelect_CharacterSelected(CharacterType type)
        {
            bool isPlantCharacter = LayerAndMaskHelper.IsTypeInMask(_characterPassingTroughMask, type);

            _normalCollider.gameObject.SetActive(!isPlantCharacter);
            _normalCollider.isTrigger = isPlantCharacter;

            StopInteracting();
        }

        #region IBurnable
        public bool CanBurnDown()
        {
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
        }
        #endregion

        #region IClimbable
        public Action NotifyOnReachedTop()
        {
            return OnCancelingClimbMode;
        }

        #endregion


        #region PoolObjectBase
        public override void ResetObject()
        {
            State = InteractableState.Normal;
            return;
        }
        #endregion

        #region IInteractable
        protected override void SetInteractableType()
        {
            interactableType = InteractableType.PlantStandAlone;
        }
        
        public override void StartInteracting()
        {
            switch (_plantDirection)
            {
                case DirectionType.Left:
                case DirectionType.Right:
                    return;
            }
            _upperGuideCollider.enabled = false;
            _bottomGuideCollider.enabled = true;

            return;
        }

        public override void StopInteracting()
        {
            switch (_plantDirection)
            {
                case DirectionType.Left:
                case DirectionType.Right:
                    return;

                case DirectionType.Up:
                    _upperGuideCollider.enabled = true;
                    _bottomGuideCollider.enabled = false;
                    break;
                case DirectionType.Down:
                    _upperGuideCollider.enabled = false;
                    _bottomGuideCollider.enabled = false;
                    break;
            }

            return;
        }

        public override bool IsInteractionAllowed(CharacterType type)
        {
            _normalCollider.isTrigger = LayerAndMaskHelper.IsTypeInMask(characterMask, type);

            switch (_plantDirection)
            {
                case DirectionType.Up:
                case DirectionType.Down:
                    break;
                case DirectionType.Left:
                case DirectionType.Right:
                    return false;
            }

            if (State != InteractableState.Normal)
                return false;

            return LayerAndMaskHelper.IsTypeInMask(characterMask, type);
        }
        #endregion

        #region InteractableBase
        protected override void BurnedBehaviourState()
        {
            BurnDown();
        }

        protected override void DestroyedBehaviourState()
        {
            SetActivity(false);
        }

        protected override void NormalBehaviourState()
        {
            return;
        }

        protected override void SproutedBehaviourState()
        {
            return;
        }

        protected override void StoneBehaviourState()
        {
            return;
        }

        #endregion
    }

}

