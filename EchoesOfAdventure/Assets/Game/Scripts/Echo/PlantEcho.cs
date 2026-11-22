
using Events;
using GameEnums;
using Helper;
using Interactable;
using Interface;
using Player;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.DebugUI;

namespace Echo
{
    [SelectionBase]
    public class PlantEcho : EchoBase, IBurnable, IClimbable, IInteractable
    {
        [Inject] private ProjectEvents projectEvents;
        [Inject] private InteractableConfigurations _interactableConfigurations;

        [SerializeField] private CharacterMask _characterMask;

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

        private DirectionType _plantDirection;
        private float _plantScale;


        private Material _plantMaterial;
        private Material _outlineMaterial;
        private Action OnCancelingClimbMode;


        private void Awake()
        {
            _plantMaterial = _renderer.material;
            _outlineMaterial = _outlineRenderer.material;
            projectEvents.Character_CharacterSelected += CharacterSelect_CharacterSelected;
        }

        private void Start()
        {
            StopInteracting();
        }

        private void OnDestroy()
        {
            projectEvents.Character_CharacterSelected -= CharacterSelect_CharacterSelected;
        }

        public void SetValues(DirectionType newRotation, float newHeight, CharacterMask mask = default)
        {
            if (newRotation == 0)
                _plantDirection = DirectionType.Up;
            else
            _plantDirection = newRotation;
            _plantScale = newHeight;

            //Rotation
            Quaternion rotation = Quaternion.identity;

            if (mask != default)
                _characterMask = mask;



            switch (_plantDirection)
            {
                case DirectionType.Up:
                    rotation = Quaternion.identity;

                    _upperGuideCollider.gameObject.SetActive(true);
                    _bottomGuideCollider.gameObject.SetActive(true);

                    _leftGuideCollider.gameObject.SetActive(false);
                    _rightGuideCollider.gameObject.SetActive(false);

                    _upperGuideCollider.enabled = true;
                    break;

                case DirectionType.Down:
                    rotation = Quaternion.Euler(0, 0, 180);

                    _upperGuideCollider.gameObject.SetActive(true);
                    _bottomGuideCollider.gameObject.SetActive(true);

                    _leftGuideCollider.gameObject.SetActive(false);
                    _rightGuideCollider.gameObject.SetActive(false);

                    _upperGuideCollider.enabled = true;
                    break;

                case DirectionType.Left:
                    rotation = Quaternion.Euler(0, 0, 90);

                    _upperGuideCollider.gameObject.SetActive(false);
                    _bottomGuideCollider.gameObject.SetActive(false);

                    _leftGuideCollider.gameObject.SetActive(false);
                    _rightGuideCollider.gameObject.SetActive(true);

                    _rightGuideCollider.enabled = true;
                    break;

                case DirectionType.Right:
                    rotation = Quaternion.Euler(0, 0, -90);

                    _upperGuideCollider.gameObject.SetActive(false);
                    _bottomGuideCollider.gameObject.SetActive(false);

                    _leftGuideCollider.gameObject.SetActive(true);
                    _rightGuideCollider.gameObject.SetActive(false);

                    _leftGuideCollider.enabled = true;
                    break;
            }


            transform.rotation = rotation;

            //Scale
            _colliderParent.localScale = new Vector3(_colliderParent.localScale.x, _plantScale, _colliderParent.localScale.z);

            SetGraphicValueOverTime();
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

        private async void SetGraphicValueOverTime()
        {
            float time = 0;

            float value = Mathf.Max(
                Constants.Values.MinimumPlantVisualHeight,
                Mathf.Min(
                    _plantScale / Constants.Values.MaximumPlantVisualHeight - (Constants.Values.MinimumPlantVisualHeight * 2),
                    Constants.Values.MaximumPlantVisualHeight));

            while (time < _interactableConfigurations.TimeForPlantEchoGrowsVisually)
            {
                float current = Mathf.Lerp(0, value,
                    _interactableConfigurations.AnimationCurveOfgrowingPlantEcho.Evaluate(time / _interactableConfigurations.TimeForPlantEchoGrowsVisually));

                _plantMaterial.SetFloat(
                    "_GrowFloat", current);

                _outlineMaterial.SetFloat(
                    "_GrowFloat", current);

                //To-Do: Maybe track if Game is paused
                time += Time.deltaTime;
                await Task.Yield();
            }

            _plantMaterial.SetFloat("_GrowFloat", value);
            _outlineMaterial.SetFloat("_GrowFloat", value);
        }

        private void CharacterSelect_CharacterSelected(CharacterType type)
        {
            bool isPlantCharacter = LayerAndMaskHelper.IsTypeInMask(_characterMask, type);

            _normalCollider.gameObject.SetActive(!isPlantCharacter);
            _normalCollider.isTrigger = isPlantCharacter;

            StopInteracting();

        }

        #region PoolObjectBase
        public override void ResetObject()
        {
            _plantDirection = 0;
            _plantScale = 0;
            //To-Do: Reset Material Value
            SetGraphicValue();
        }
        #endregion

        #region EchoBase
        protected override void NormalStateBehaviour()
        {
            return;
        }

        protected override void DestroyedStateBehaviour()
        {
            SetActivity(false);
        }

        protected override void BurnedStateBehaviour()
        {
            BurnDown();
        }
        #endregion

        #region IBurnable
        public bool CanBurnDown()
        {
            switch (State)
            {
                case EchoState.Normal:
                    return true;
            }
            return false;
        }

        public void BurnDown()
        {
            State = EchoState.Burned;

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

        #region IInteractable

        public InteractableType GetInteractableType()
        {
            throw new NotImplementedException();
        }

        public bool IsInteractionAllowed(CharacterType type)
        {
            _normalCollider.isTrigger = LayerAndMaskHelper.IsTypeInMask(_characterMask, type);

            switch (_plantDirection)
            {
                case DirectionType.Up:
                case DirectionType.Down:
                    break;
                case DirectionType.Left:
                case DirectionType.Right:
                    return false;
            }

            if (State != EchoState.Normal)
                return false;

            return LayerAndMaskHelper.IsTypeInMask(_characterMask, type);
        }

        public void StartInteracting()
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

        public void StopInteracting()
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
        #endregion
    }
}