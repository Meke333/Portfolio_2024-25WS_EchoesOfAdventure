using System;
using System.Collections;
using Events;
using Interface;
using Player;
using SceneLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class LevelManager : MonoBehaviour
{

    [Inject] private ProjectEvents _projectEvents;

    private const int MAXRUN = 2;

    private int currentRun = 0;

    private float resetHoldTime = 1.0f; // Duration to consider a hold press
    private float resetPressStartTime; // Tracks when the button was first pressed
    private bool isResetButtonHeld = false; // Tracks if the button is currently held
    private bool hasTriggeredHoldAction = false; // Prevents multiple triggers of the hold action
    private bool _screenIsOpen;
    private bool _isSaveSystemActive;

    private bool _isProcessingLevelEnd;

    [SerializeField] private LayerMask _playerLayer;

    [SerializeField] private Transform _hitbox; // Assign your hitbox in the inspector or programmatically
    [SerializeField] private GameObject _VFX;
    [SerializeField] private GameObject _blocker;

    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private SceneSetups _nextScene;

    private void OnEnable()
    {
        if (_projectEvents != null)
        {
            _projectEvents.LoadRequested += LoadToRun;
            _projectEvents.FinishedLevel += ActivateGoal;
            _projectEvents.ScreenClosed += ScreenClosedOuside;
            _projectEvents.LoadToCurrentRun += ResetCurrentRun;
            _projectEvents.LevelStart += LevelStart;
            _projectEvents.ReadyToSwitchLevel += SwitchLevel;

        }

        InputSystem.actions["Reset"].performed += OnResetPressed;
        InputSystem.actions["Reset"].canceled += OnResetReleased;
    }

    private void OnDisable()
    {
        if (_projectEvents != null)
        {
            _projectEvents.LoadRequested -= LoadToRun;
            _projectEvents.FinishedLevel -= ActivateGoal;
            _projectEvents.ScreenClosed -= ScreenClosedOuside;
            _projectEvents.LoadToCurrentRun -= ResetCurrentRun;
            _projectEvents.LevelStart -= LevelStart;
            _projectEvents.ReadyToSwitchLevel -= SwitchLevel;
        }

        InputSystem.actions["Reset"].performed -= OnResetPressed;
        InputSystem.actions["Reset"].canceled -= OnResetReleased;
    }

    private void FixedUpdate()
    {
        CheckForPlayer();
    }

    private void Update()
    {
        // Check if the reset button is held and enough time has passed
        if (isResetButtonHeld && !hasTriggeredHoldAction)
        {
            if (Time.time - resetPressStartTime >= resetHoldTime)
            {
                ResetCurrentRun(); // Trigger the hold action
                hasTriggeredHoldAction = true; // Prevent re-triggering
            }
        }
    }

    private void CheckForPlayer()
    {
        // Ensure the _hitbox object is active
        if (_hitbox == null || !_hitbox.gameObject.activeInHierarchy)
        {
            return;
        }

        // Use Collider2DHelper to detect the player within the hitbox
        IHurtBox[] playerHits = Collider2DHelper.GetHitArray<IHurtBox>(_hitbox, _playerLayer);

        if (playerHits.Length > 0)
        {
            if (!_isProcessingLevelEnd)
                ProcessLevelEnd();
        }
    }

    private void LevelStart()
    {
        currentRun = 0;
        _projectEvents.SaveRequested?.Invoke(currentRun);
        _projectEvents.OpenSelectionScreen?.Invoke(currentRun);
    }

    private void ResetCurrentRun()
    {
        _projectEvents.DeactivateCurrentCharacter?.Invoke();
        ResetGoal();

        _projectEvents.Character_RequestCharacterSwitch?.Invoke(_playerManager.CurrentPlayer.CharacterType);
        _projectEvents.LoadRequested?.Invoke(currentRun);
    }

    private void OpenScreen()
    {

        //Deactivate PlayerMovement

        // Notify the selection screen to open
        _projectEvents.OpenSelectionScreen?.Invoke(currentRun);

        _screenIsOpen = true;
    }

    private void CloseScreen()
    {
        // Notify the selection screen to close
        _projectEvents.CloseSelectionScreen?.Invoke();
        _screenIsOpen = false;
    }

    private void ScreenClosedOuside()
    {
        _screenIsOpen = false;
    }

    private void ProcessLevelEnd()
    {
        _isProcessingLevelEnd = true;
        Debug.Log("Finished Level!");
        _projectEvents.DeactivateCurrentCharacter?.Invoke();

        currentRun++;

        if (currentRun > MAXRUN)
        {
            _projectEvents.OpenLevelEndScreen?.Invoke();
            _projectEvents.Character_DeactivateAllCharacters?.Invoke();
            _projectEvents.Interactions_DeactivateAllInteracttions?.Invoke();
            return;
        }

        _projectEvents.SaveRequested(currentRun);
        _projectEvents.OpenSelectionScreen?.Invoke(currentRun);
        ResetGoal();
    }

    private void SwitchLevel()
    {
        _projectEvents.LoadToLevelByScene?.Invoke(_nextScene);
    }

    private void LoadToRun(int loadToRun)
    {
        currentRun = loadToRun;
        ResetGoal();
    }

    private void ActivateGoal()
    {
        _hitbox.gameObject.SetActive(true);
        _VFX.gameObject.SetActive(true);
        _blocker.gameObject.SetActive(false);
    }

    private void ResetGoal()
    {
        _hitbox.gameObject.SetActive(false);
        _VFX.gameObject.SetActive(false);
        _projectEvents.ResetGoal?.Invoke();
        _blocker.gameObject.SetActive(true);

        _isProcessingLevelEnd = false;
    }

    private void OnResetPressed(InputAction.CallbackContext context)
    {
        resetPressStartTime = Time.time; // Record the time when the button was pressed
        isResetButtonHeld = true; // Set the flag to true
        hasTriggeredHoldAction = false; // Reset the triggered action flag
    }

    private void OnResetReleased(InputAction.CallbackContext context)
    {
        isResetButtonHeld = false; // Reset the flag

        // If the hold action wasn't triggered, consider it a short press
        if (!hasTriggeredHoldAction && !_screenIsOpen)
        {
            OpenScreen();

        }
        else if (_screenIsOpen)
        {
            CloseScreen();
        }
    }
}
