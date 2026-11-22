
using Events;
using Helper;
using Interface;
using NaughtyAttributes;
using Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Interactable
{

    [SelectionBase]
    public class Waterfall : MonoBehaviour, IInteractable, IClimbable
    {
        [Inject] private InteractableConfigurations _InteractableConfigurations;
        [Inject] private ProjectEvents _projectEvents;

        [ShowNonSerializedField] private InteractableType _interactableType = InteractableType.Ladder;
        [SerializeField] private CharacterMask _interactableCharacterMask;

        [SerializeField] private Collider2D _mainCollider;
        [SerializeField] private Collider2D _upperGuideCollider;
        [SerializeField] private Collider2D _bottomGuideCollider;

        [SerializeField] private Transform _colliderParent;
        private Vector3 _hazardPosition;
        private Vector3 _hazardScale;

        private IHurtBox[] _savedList = default;


        private float _time;
        private float time
        {
            get => _time;
            set
            {
                if (value < 0)
                { 
                    return;
                }
                else if (value >= _InteractableConfigurations.TimeBeforeWaterHurtsFireCharacter)
                {
                    HurtPlayer();
                    return;
                }
                _time = value;
            }
        }

        private bool _isDrainingTime;
        //TO-DO: Speak with Ben about Character Switching
        [SerializeField] private CharacterType _characterType; //DEBUG ONLY!! 

        private void CheckForPlayer()
        {
            if (_characterType != CharacterType.Fire)
                return;

            IHurtBox[] list = Collider2DHelper.GetHitArray<IHurtBox>(_hazardPosition, _hazardScale, Constants.Layers.Player);

            _savedList = list;

            _isDrainingTime = list.Length > 0;

        }
        private void HurtPlayer()
        {
            for (int i = 0; i < _savedList.Length; i++)
            {
                _savedList[i].GetDamage();
            }
            //To-Do: Talk with Ben about Level Events Dying!
            Debug.Log("Player should drown now!");
        }

        #region UnityMethods
        private void Awake()
        {
            _projectEvents.Character_CharacterSelected += CharacterSelect_CharacterSelected;
            _hazardPosition = new Vector3(_colliderParent.position.x, _colliderParent.position.y + _colliderParent.localScale.y / 2);
            _hazardScale = _colliderParent.localScale;
        }

        private void Start()
        {
            StopInteracting();
        }

        private void OnDestroy()
        {
            _projectEvents.Character_CharacterSelected -= CharacterSelect_CharacterSelected;
        }

        private void Update()
        {
            time += _isDrainingTime ? Time.deltaTime : -Time.deltaTime;
        }

        private void FixedUpdate()
        {
            CheckForPlayer();
        }

        #endregion

        #region IInteractable
        public InteractableType GetInteractableType()
        {
            return InteractableType.Ladder;
        }

        public bool IsInteractionAllowed(CharacterType type)
        {
            return LayerAndMaskHelper.IsTypeInMask(_interactableCharacterMask, type);
        }

        public void StartInteracting()
        {
            // _upperGuideCollider.enabled = false;
            _bottomGuideCollider.enabled = true;
        }

        public void StopInteracting()
        {
            //_upperGuideCollider.enabled = true;
            _bottomGuideCollider.enabled = false;
        }
        #endregion

        #region IClimbable
        public Action NotifyOnReachedTop()
        {
            return null;
        }
        #endregion

        #region Events

        private void CharacterSelect_CharacterSelected(CharacterType type)
        {
            if (_characterType == type)
                return;

            switch (type)
            {
                case CharacterType.Fire:
                    _mainCollider.isTrigger = true;
                    break;
                case CharacterType.Plant:
                    _mainCollider.isTrigger = false;
                    break;
                case CharacterType.Stone:
                    _mainCollider.isTrigger = true;
                    break;
            }

            _characterType = type;

            StopInteracting();
        }
        #endregion
    }
}