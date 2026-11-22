using Echo;
using GameEnums;
using Helper;
using Interface;
using NaughtyAttributes;
using Player;
using UnityEngine;
using Zenject;

namespace Interactable
{
    public class PlantSproutInteractable : InteractableBase, IBurnable, ISproutable
    {
        [SerializeField] private DirectionType _plantDirection;
        [SerializeField, Range(1, Constants.Values.MaximumPlantVisualHeight)] private float _plantScale = 1f;
        [SerializeField] private CharacterMask _characterPassingTroughMask;

#if UNITY_EDITOR
        [SerializeField, Foldout("Debug")] private Transform _debug_scaleIndicator;

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

            _debug_scaleIndicator.localScale = new Vector3(_debug_scaleIndicator.localScale.x, _plantScale, _debug_scaleIndicator.localScale.z);
        }

#endif

        #region InteractableBase
        protected override void NormalBehaviourState()
        {
            return;
        }

        protected override void DestroyedBehaviourState()
        {
            SetActivity(false);
        }

        protected override void BurnedBehaviourState()
        {
            BurnDown();
        }

        protected override void SproutedBehaviourState()
        {
            Debug.Log("Sprouted");

            Sprout();
        }

        protected override void StoneBehaviourState()
        {
            return;
        }
        #endregion

        #region IPoolObject

        public override void ResetObject()
        {
            //State = InteractableState.Normal;
            return;
        }
        #endregion

        #region IInteractable
        protected override void SetInteractableType()
        {
            interactableType = InteractableType.PlantSprout;
        }

        public override void StartInteracting()
        {
            throw new System.NotImplementedException();
        }

        public override void StopInteracting()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsInteractionAllowed(CharacterType type)
        {
            switch (State)
            {
                case InteractableState.Destroyed:
                case InteractableState.Burned:
                case InteractableState.Sprouted:
                case InteractableState.Stone:
                    return false;
            }

            return LayerAndMaskHelper.IsTypeInMask(characterMask, type);
        }
        #endregion

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

        #region ISproutable

        public void Sprout()
        {
            State = InteractableState.Sprouted;

            PlantEcho plantEcho = poolManager.ActivateInOtherManager(typeof(PlantEchoSpawner).GetHashCode(), transform.position).GetComponent<PlantEcho>();
            plantEcho.SetValues(_plantDirection, _plantScale, _characterPassingTroughMask);

            SetActivity(false);
        }


        #endregion


    }
}

