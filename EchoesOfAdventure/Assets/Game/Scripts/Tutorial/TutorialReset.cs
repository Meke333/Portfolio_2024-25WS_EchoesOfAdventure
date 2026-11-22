using System;
using Events;
using Helper;
using Interface;
using Player;
using UnityEngine;
using Zenject;

public class TutorialReset : MonoBehaviour
{
    [Inject] private ProjectEvents _projectEvents;

    [SerializeField] private LayerMask _playerLayer;

    [SerializeField] private Transform _collider; // Assign your hitbox in the inspector or programmatically;

    private bool tutorialIsFinished = false;


    private CharacterType _characterType;

    private void OnEnable()
    {
        _projectEvents.Character_CharacterSelected += CharacterSelect_CharacterSelected;
    }

    private void OnDisable()
    {
        _projectEvents.Character_CharacterSelected -= CharacterSelect_CharacterSelected;
    }

    private void FixedUpdate()
    {

        if (!tutorialIsFinished)
        {
            CheckForPlayer();
        }
    }

    private void CheckForPlayer()
    {
        // Ensure the _hitbox object is active
        if (_collider == null || !_collider.gameObject.activeInHierarchy)
        {
            return;
        }

        // Use Collider2DHelper to detect the player within the hitbox
        IHurtBox[] playerHits = Collider2DHelper.GetHitArray<IHurtBox>(_collider, _playerLayer);

        if (playerHits.Length > 0)
        {
            ActivateLastTutorialStep();
        }
    }
    
    private void CharacterSelect_CharacterSelected(CharacterType type)
    {
        if (_characterType == type)
            return;

        switch (type)
        {
            case CharacterType.Fire:
                _collider.gameObject.SetActive(false);
                break;
            case CharacterType.Plant:
                _collider.gameObject.SetActive(false);
                break;
            case CharacterType.Stone:
                _collider.gameObject.SetActive(true);
                break;
        }

        _characterType = type;


    }

    private void ActivateLastTutorialStep()
    {
        tutorialIsFinished = true;
        _projectEvents.FinishedTutorial?.Invoke();
    }
}
