using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Events;
using Interactable;
using Player;
using UnityEngine.UI;
using Zenject;

public class InGameUI : MonoBehaviour
{
    
    [Inject] private ProjectEvents _projectEvents;
    public enum UIState
    {
        Active,
        Hidden
    }

    [Header("UI Animator")]
    [SerializeField] private Animator _animator;

    [Header("UI Visibility")]
    [SerializeField] private float idleDelay = 2f; // Delay before showing when idle
    [SerializeField] private float moveDelay = 2f; // Delay before hiding when moving
    
    [Header("Run Indicator")]
    public RectTransform runIndicator;
    public float[] runIndicatorPositions = { -420f, 0f, 420f };

    [Header("RunImages")] 
    public Image[] runImages;
    
    [Header("CharacterImages")]
    public Sprite fireSprite;
    public Sprite stoneSprite;
    public Sprite plantSprite;
    public Sprite neutralImage;
    
    [Header("Collectible")]
    public Image collectibleImage;
    public Sprite fireCollectibeSprite;
    public Sprite stoneCollectibleSprite;
    public Sprite plantCollectibleSprite;
    public Sprite notCollectedSprite;

    private Dictionary<CharacterType, Sprite> characterRunSpriteMap;
    private Dictionary<CharacterType, Sprite> characterCollectibleSpriteMap;
    
    private InputAction moveAction;
    private Coroutine currentCoroutine;
    private bool isMoving;
    private UIState currentState;
    private CharacterType _currentCharacterType;

    private void Start()
    {
        // Initialize UI state
        currentState = UIState.Hidden;
        
    }
    
    void Awake()
    {
        // Initialize the dictionary with the sprites
        characterRunSpriteMap = new Dictionary<CharacterType, Sprite>
        {
            { CharacterType.Fire, fireSprite },
            { CharacterType.Stone, stoneSprite },
            { CharacterType.Plant, plantSprite }
        };
        
        // Initialize the dictionary with the sprites
        characterCollectibleSpriteMap = new Dictionary<CharacterType, Sprite>
        {
            { CharacterType.Fire, fireCollectibeSprite },
            { CharacterType.Stone, stoneCollectibleSprite },
            { CharacterType.Plant, plantCollectibleSprite }
        };
    }

    private void OnEnable()
    {
        // Reference the Move action by name from the Input System
        moveAction = InputSystem.actions["Move"];

        if (moveAction != null)
        {
            // Subscribe to the Move action events
            moveAction.performed += OnMoveInput;
            moveAction.canceled += OnMoveInput;
        }
        else
        {
            Debug.LogError("Move action not found in Input System!");
        }

        _projectEvents.UIChangeRunAndCharacterInformation += ChangeRunAndCharacterInformation;
        _projectEvents.ResetGoal += ResetCollectibleImage;
        _projectEvents.UICollectibleCollected += SetCollectibleImage;
        _projectEvents.UISetInGameState += SetUIState;
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            // Unsubscribe from the Move action events
            moveAction.performed -= OnMoveInput;
            moveAction.canceled -= OnMoveInput;
        }
        
        _projectEvents.UIChangeRunAndCharacterInformation -= ChangeRunAndCharacterInformation;
        _projectEvents.ResetGoal -= ResetCollectibleImage;
        _projectEvents.UICollectibleCollected -= SetCollectibleImage;
        _projectEvents.UISetInGameState -= SetUIState;
    }

    private void ChangeRunAndCharacterInformation(int currentRun, CharacterType characterType)
    {
        // Set the position of the run indicator
        runIndicator.anchoredPosition = new Vector2(runIndicatorPositions[currentRun], runIndicator.anchoredPosition.y);

        _currentCharacterType = characterType;

        // Update the sprite for the current run
        if (characterRunSpriteMap.TryGetValue(_currentCharacterType, out Sprite sprite))
        {
            runImages[currentRun].sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"No sprite found for CharacterType: {characterType}");
        }

        // Set the sprites of run buttons above the current run to the neutral sprite
        for (int i = currentRun + 1; i < runImages.Length; i++)
        {
            runImages[i].sprite = neutralImage;
        }
    }
    
    private void ResetCollectibleImage()
    {
        collectibleImage.sprite = notCollectedSprite;
    }
    
    private void SetCollectibleImage()
    {
        // Update the sprite for the current run
        if (characterCollectibleSpriteMap.TryGetValue(_currentCharacterType, out Sprite sprite))
        {
            _animator.SetTrigger(Constants.AnimatorParameters.InGameUI_CollectibleCollected);
            collectibleImage.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"No sprite found for CharacterType: {_currentCharacterType}");
        }
    }
    

    #region UI States & Activation
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        // Check if there is movement input
        Vector2 moveInput = context.ReadValue<Vector2>();
        bool playerMoving = moveInput.sqrMagnitude > 0;

        if (playerMoving != isMoving)
        {
            isMoving = playerMoving;

            currentCoroutine = StartCoroutine(isMoving ? HideUIAfterDelay() : ShowUIAfterDelay());
        }
    }

    private IEnumerator ShowUIAfterDelay()
    {
        yield return new WaitForSeconds(idleDelay); // Wait before showing UI

        if (!isMoving)
            SetUIState(UIState.Active);
    }

    private IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(moveDelay); // Wait before hiding UI

        if (isMoving)
            SetUIState(UIState.Hidden);
    }

    public void SetUIState(UIState state)
    {
        // Avoid unnecessary state transitions
        if (currentState == state)
            return;

        currentState = state;

        if (currentState == UIState.Active)
        {
            //uiElement.SetActive(true); // Ensure UI is active for animation
            //StartCoroutine(AnimateScale(uiElement, visibleScale, animationDuration));

            ToggleAnimatorState(true);
        }
        else if (currentState == UIState.Hidden)
        {
            //StartCoroutine(AnimateScale(uiElement, hiddenScale, animationDuration));
            //StartCoroutine(DisableUIAfterAnimation());


            ToggleAnimatorState(false);
        }
    }

    private void ToggleAnimatorState(bool isShowing)
    {
        _animator.SetBool(Constants.AnimatorParameters.InGameUI_IsShowing, isShowing);
    }

    public UIState GetUIState()
    {
        return currentState;
    }
    
    #endregion
    
}
