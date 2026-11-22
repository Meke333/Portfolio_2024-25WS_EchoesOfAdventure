using System;
using Events;
using SceneLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton; // Reference to the Start button
    [SerializeField] private Button quitButton;  // Reference to the Quit button
    [SerializeField] private Button levelSelectButton;  // Reference to the Quit button
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;

    [SerializeField] private GameObject levelSelectScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;

    [Inject] private ProjectEvents _projectEvents; // Inject your project events
    
    [SerializeField] private SceneSetups _firstLevel;
    
    private int _selectedButtonIndex = 0; // Keeps track of the currently selected button
    private Button[] _buttons;           // Array to hold the buttons in the menu
    
    private GameObject _lastSelectedButton; // Tracks the last selected button
    
    private void Awake()
    {
        _selectedButtonIndex = 0;
        // Highlight the first button
        SelectButton(_selectedButtonIndex);
        AssignButtonCallbacks();
        InitializeButtons();
        
        _projectEvents.LevelStart?.Invoke();
    }

    private void OnEnable()
    {
        _selectedButtonIndex = 0;
        // Highlight the first button
        SelectButton(_selectedButtonIndex);
    }

    private void Update()
    {
        HandleButtonReselection();
    }
    
    /// <summary>
    /// Handles the reselection of buttons when using the D-Pad or joystick.
    /// </summary>
    private void HandleButtonReselection()
    {
        // Check if no button is currently selected
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Detect input from the D-Pad or joystick
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                // Re-select the last selected button or the default button
                var buttonToSelect = _lastSelectedButton != null
                    ? _lastSelectedButton
                    : _buttons[_selectedButtonIndex].gameObject;

                EventSystem.current.SetSelectedGameObject(buttonToSelect);
            }
        }
        else
        {
            // Update the last selected button
            _lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }

    /// <summary>
    /// Assigns callbacks to buttons using serialized fields.
    /// </summary>
    private void AssignButtonCallbacks()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Start Button is not assigned in the inspector!");
        }
        
        if (levelSelectButton != null)
        {
            levelSelectButton.onClick.AddListener(OpenLevelSelectScreen);
        }
        else
        {
            Debug.LogError("Level Select Button is not assigned in the inspector!");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogError("Quit Button is not assigned in the inspector!");
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(OpenOptionsScreen);
        }

        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(OpenCreditsScreen);
        }
    }

    /// <summary>
    /// Initializes the button array and sets up the first selected button.
    /// </summary>
    private void InitializeButtons()
    {
        _buttons = new[] { startButton, levelSelectButton, quitButton, creditsButton };

        if (_buttons.Length > 0)
        {
            // Ensure the first button is selected
            SelectButton(_selectedButtonIndex);
        }
        else
        {
            Debug.LogError("No buttons assigned in the menu!");
        }
    }
    
    

    /// <summary>
    /// Highlights the button at the given index.
    /// </summary>
    /// <param name="index">Index of the button to select.</param>
    private void SelectButton(int index)
    {
        if (_buttons == null || _buttons.Length == 0) return;

        // Highlight the selected button
        _buttons[index].Select();
    }


    /// <summary>
    /// Starts the game by loading Level 1 (index 1 in SceneSetups).
    /// </summary>
    private void StartGame()
    {
        Debug.Log("Button: " + _projectEvents);
        Debug.Log("Start Game clicked. Loading Level 1...");
        _projectEvents.LoadToLevelByScene?.Invoke(_firstLevel);

    }

    /// <summary>
    /// Quits the game application.
    /// </summary>
    private void QuitGame()
    {
        Debug.Log("Quit Game clicked.");
        Application.Quit();
    }

    private void OpenLevelSelectScreen()
    {
        levelSelectScreen.SetActive(true);
        gameObject.SetActive(false);
    }
    
    private void OpenOptionsScreen()
    {
        optionsScreen.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OpenCreditsScreen()
    {
        creditsScreen.SetActive(true);
        gameObject.SetActive(false);
    }
}