using System;
using System.Collections.Generic;
using Events;
using Player;
using SceneLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class LevelEndScreen : MonoBehaviour
{
    [Inject] private ProjectEvents _projectEvents;

    [Header("Buttons")]
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button levelScreenButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Screens")]
    [SerializeField] private GameObject levelEndScreen;
    [SerializeField] private GameObject levelSelectionScreen;
    
    // How long (in seconds) we wait between navigation inputs
    [SerializeField] private float _navigationCooldown = 0.2f;

// Time at which we last navigated
    private float _lastNavTime;


    // or if you have a static reference you might skip this.
    
    private int _selectedButtonIndex = 0;
    private Button[] _buttons;
    
    private Dictionary<CharacterType, Sprite> _characterRunSpriteMap;
    private Dictionary<CharacterType, Sprite> _artifactRunSpriteMap;
    
    private CharacterType _currentCharacterType;
    
    [Header("CharacterImages")]
    public Sprite fireSprite;
    public Sprite stoneSprite;
    public Sprite plantSprite;
    
    [Header("Artifact Images")]
    public Sprite fireArtifactSprite;
    public Sprite stoneArtifactSprite;
    public Sprite plantArtifacSprite;
    
    [Header("RunImages")] 
    public Image[] runImages;
    public Image[] artifactImages;

    private void OnEnable()
    {
        _projectEvents.OpenLevelEndScreen += OpenLevelEndScreen;
        _projectEvents.ResetGoal += ResetLevel;

        // Set up input listeners
        InputSystem.actions["CharacterSelectionUp"].started += OnScrollUp;
        InputSystem.actions["CharacterSelectionDown"].started += OnScrollDown;
        _projectEvents.UIChangeRunAndCharacterInformation += ChangeRunAndCharacterInformation;
    }

    private void ResetLevel()
    {
        levelEndScreen.SetActive(false);
        levelSelectionScreen.SetActive(false);
    }

    private void OnDisable()
    {
        _projectEvents.OpenLevelEndScreen -= OpenLevelEndScreen;
        _projectEvents.ResetGoal -= ResetLevel;

        // Set up input listeners
        InputSystem.actions["CharacterSelectionUp"].started -= OnScrollUp;
        InputSystem.actions["CharacterSelectionDown"].started -= OnScrollDown;
        _projectEvents.UIChangeRunAndCharacterInformation -= ChangeRunAndCharacterInformation;
    }

    private void Awake()
    {
        // Initialize the dictionary with the sprites
        _characterRunSpriteMap = new Dictionary<CharacterType, Sprite>
        {
            { CharacterType.Fire, fireSprite },
            { CharacterType.Stone, stoneSprite },
            { CharacterType.Plant, plantSprite }
        };
        
        _artifactRunSpriteMap = new Dictionary<CharacterType, Sprite>
        {
            { CharacterType.Fire, fireArtifactSprite },
            { CharacterType.Stone, stoneArtifactSprite },
            { CharacterType.Plant, plantArtifacSprite }
        };
    }

    private void Update()
    {
        if (levelEndScreen.activeSelf)
            SelectButton(_selectedButtonIndex);
    }

    private void OpenLevelEndScreen()
    {
        levelEndScreen.SetActive(true);
        AssignButtonCallbacks();
        
        InitializeButtons();
        SelectButton(_selectedButtonIndex);
    }

    private void InitializeButtons()
    {
        _buttons = new[] { nextLevelButton, levelScreenButton, mainMenuButton };

        if (_buttons.Length > 0)
        {
            SelectButton(_selectedButtonIndex);
        }
    }

    private void SelectButton(int index)
    {
        if (_buttons == null || _buttons.Length == 0) return;
        
        // Make sure to set the selection via the EventSystem
        _buttons[index].Select();
        EventSystem.current.SetSelectedGameObject(_buttons[index].gameObject);
    }

    private void OnScrollUp(InputAction.CallbackContext ctx)
    {
        // Only navigate if the screen is active
        if (!levelEndScreen.activeSelf) return;
    
        // Check the grace period
        if (Time.time - _lastNavTime < _navigationCooldown)
        {
            // Too soon; ignore this input
            return;
        }

        // If we get here, enough time has passed, so we can process the input.
        _lastNavTime = Time.time; // Reset the timer to now
    
        // Now do your normal logic
        _selectedButtonIndex--;
        if (_selectedButtonIndex < 0)
        {
            _selectedButtonIndex = _buttons.Length - 1;
        }
    }

    private void OnScrollDown(InputAction.CallbackContext ctx)
    {
        if (!levelEndScreen.activeSelf) return;
    
        // Same cooldown check
        if (Time.time - _lastNavTime < _navigationCooldown)
        {
            return;
        }
        _lastNavTime = Time.time;

        // Proceed with normal logic
        _selectedButtonIndex++;
        if (_selectedButtonIndex >= _buttons.Length)
        {
            _selectedButtonIndex = 0;
        }
    }


    private void AssignButtonCallbacks()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(StartNextLevel);
        }
        
        if (levelScreenButton != null)
        {
            levelScreenButton.onClick.AddListener(OpenLevelSelectScreen);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OpenMainMenu);
        }
    }
    
    private void StartNextLevel()
    {
        _projectEvents.PlayerFinishedLevel?.Invoke();
    }
    
    private void OpenLevelSelectScreen()
    {
        levelSelectionScreen.SetActive(true);
        levelEndScreen.SetActive(false);
    }
    
    private void OpenMainMenu()
    {
        _projectEvents.LoadToLevelByScene(SceneSetups.MainMenu);
    }
    
    #region Character Display

    private void ChangeRunAndCharacterInformation(int currentRun, CharacterType characterType)
    {
        _currentCharacterType = characterType;

        // Update the sprite for the current run
        if (_characterRunSpriteMap.TryGetValue(_currentCharacterType, out Sprite characterSprite))
        {
            runImages[currentRun].sprite = characterSprite;
        }
        else
        {
            Debug.LogWarning($"No sprite found for CharacterType: {characterType}");
        }
        
        // Update the sprite for the current run
        if (_artifactRunSpriteMap.TryGetValue(_currentCharacterType, out Sprite artifactSprite))
        {
            artifactImages[currentRun].sprite = artifactSprite;
        }
        else
        {
            Debug.LogWarning($"No sprite found for CharacterType: {characterType}");
        }
    }

    #endregion
}
