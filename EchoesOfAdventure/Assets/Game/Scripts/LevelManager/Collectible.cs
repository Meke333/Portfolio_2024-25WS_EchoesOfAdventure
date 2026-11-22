using System;
using Events;
using FMODUnity;
using Interactable;
using Interface;
using MirzaBeig.ParticleSystems;
using Player;
using UnityEngine;
using Zenject;

public class Collectible : MonoBehaviour
{
    [Inject] private ProjectEvents _projectEvents;
    [Inject] private InteractableEvents _interactableEvents;

    [SerializeField] private LayerMask _playerLayer;

    [SerializeField] private Transform _collectibleObject; // Assign your hitbox in the inspector or programmatically;

    [field: Header("Sounds")]
    [field: SerializeField] private GameObject _soundEmitterGameObject;
    [field: SerializeField] private StudioEventEmitter _soundEmitter;

    [field: Header("Graphics")]
    [field: SerializeField] private GameObject _plantRuneGameObject;
    [field: SerializeField] private GameObject _stoneRuneGameObject;
    [field: SerializeField] private GameObject _fireRuneGameObject;

    [field: Header("VFX")]
    [field: SerializeField] private ParticleSystem _particleSystem;

    private void OnEnable()
    {
        _projectEvents.ResetGoal += ResetCollectible;
        _projectEvents.Character_CharacterSelected += CharacterSelected;
    }


    private void OnDisable()
    {
        _projectEvents.ResetGoal -= ResetCollectible;
        _projectEvents.Character_CharacterSelected -= CharacterSelected;
    }

    private void FixedUpdate()
    {
        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        // Ensure the _hitbox object is active
        if (_collectibleObject == null || !_collectibleObject.gameObject.activeInHierarchy)
        {
            return;
        }

        // Use Collider2DHelper to detect the player within the hitbox
        IHurtBox[] playerHits = Collider2DHelper.GetHitArray<IHurtBox>(_collectibleObject, _playerLayer);

        if (playerHits.Length > 0)
        {
            ActivateGoal();
        }
    }

    private void ActivateGoal()
    {
        _collectibleObject.gameObject.SetActive(false);
        _soundEmitterGameObject.gameObject.SetActive(false);
        _soundEmitter.Stop();
        _projectEvents.FinishedLevel();

        _interactableEvents.Collectible_Collected?.Invoke(transform.position);
        _projectEvents.UICollectibleCollected?.Invoke();

        ToggleVFX(false);
    }

    private void ResetCollectible()
    {
        _collectibleObject.gameObject.SetActive(true);
        _soundEmitterGameObject.gameObject.SetActive(true);
        _soundEmitter.Play();

        ToggleVFX(true);
    }

    private void CharacterSelected(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Fire:
                _plantRuneGameObject.SetActive(false);
                _stoneRuneGameObject.SetActive(false);
                _fireRuneGameObject.SetActive(true);
                break;
            case CharacterType.Stone:
                _plantRuneGameObject.SetActive(false);
                _stoneRuneGameObject.SetActive(true);
                _fireRuneGameObject.SetActive(false);
                break;
            case CharacterType.Plant:
                _plantRuneGameObject.SetActive(true);
                _stoneRuneGameObject.SetActive(false);
                _fireRuneGameObject.SetActive(false);
                break;
        }
    }

    private void ToggleVFX(bool isOn)
    {
        if (_particleSystem == null)
            return;

        if (isOn)
        {
            _particleSystem.gameObject.SetActive(true);
            _particleSystem.Play(isOn);
        }
        else
            _particleSystem.Stop();
    }

}
