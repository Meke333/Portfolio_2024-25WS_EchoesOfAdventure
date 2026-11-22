using System;
using System.Collections.Generic;
using Events;
using Player;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Zenject;

[Obsolete]
public class CharacterRunSelection : MonoBehaviour
{
    
    [Inject] private ProjectEvents _projectEvents;
    
    [Header("Run Buttons")]
    public Button runButton1;
    public Button runButton2;
    public Button runButton3;

    [Header("Run Indicator")]
    public RectTransform runIndicator;
    public float[] runIndicatorPositions = { -420f, 0f, 420f };

    [Header("Character Buttons")]
    public Button plantCharacterButton;
    public Button stoneCharacterButton;
    public Button fireCharacterButton;

    private int _currentRun = 0; // Tracks the current run (0 = Run 0, 1 = Run 1, 2 = Run 2)
    private bool _isSaveSystemActive = false;

    [Header("Screens")]
    public GameObject selectionScreen;

    [Header("Tutorial")] 
    public bool isTutorial = false;

    public bool isWaterTutorial = false;

    public GameObject ResetIndicator;

    private int _tutorialStep = 0;

    private bool _canClose = true;
    
    private GameObject _lastSelectedButton; // Tracks the last selected button
    private Button[] _allButtons; // Array to hold all selectable buttons
    
    public int CurrentRun
    {
        get => _currentRun;
        set => _currentRun = value;
    }
    
    private Dictionary<int, string> runCharacterSelections = new Dictionary<int, string>(); // Tracks character selections per run

    void Start()
    {
        InitializeUI();

        // Add listeners to run buttons
        runButton1.onClick.AddListener(() => OnRunButtonPressed(0));
        runButton2.onClick.AddListener(() => OnRunButtonPressed(1));
        runButton3.onClick.AddListener(() => OnRunButtonPressed(2));

        // Add listeners to character buttons
        plantCharacterButton.onClick.AddListener(() => OnCharacterButtonPressed("Plant"));
        stoneCharacterButton.onClick.AddListener(() => OnCharacterButtonPressed("Stone"));
        fireCharacterButton.onClick.AddListener(() => OnCharacterButtonPressed("Fire"));
    }

    private void OnEnable()
    {
        _projectEvents.OpenSelectionScreen += OpenScreen;
        _projectEvents.ResetCurrentRun += ResetToRun;
        _projectEvents.FinishedTutorial += FinishTutorial;
        _projectEvents.CloseSelectionScreen += CloseScreen;
    }

    private void OnDisable()
    {
        _projectEvents.OpenSelectionScreen -= OpenScreen;
        _projectEvents.ResetCurrentRun -= ResetToRun;
        _projectEvents.FinishedTutorial -= FinishTutorial;
        _projectEvents.CloseSelectionScreen -= CloseScreen;
    }
    
    void Update()
    {
        HandleButtonReselection();
    }

    private void InitializeUI()
    {
        UpdateRunIndicator();
        UpdateRunButtons();
        UpdateCharacterButtons();
        InitializeAllButtons();
        
        // Select the first interactable button
        SelectButton(0);
    }

    private void UpdateRunIndicator()
    {
        runIndicator.anchoredPosition = new Vector2(runIndicatorPositions[_currentRun], runIndicator.anchoredPosition.y);
    }

    private void InitializeAllButtons()
    {
        // Create a list to store active and interactable buttons
        List<Button> activeButtons = new List<Button>();

        // Add Character Buttons first if they are interactable
        if (plantCharacterButton.interactable) activeButtons.Add(plantCharacterButton);
        if (stoneCharacterButton.interactable) activeButtons.Add(stoneCharacterButton);
        if (fireCharacterButton.interactable) activeButtons.Add(fireCharacterButton);

        // Add Run Buttons next if they are interactable
        if (runButton1.interactable) activeButtons.Add(runButton1);
        if (runButton2.interactable) activeButtons.Add(runButton2);
        if (runButton3.interactable) activeButtons.Add(runButton3);

        // Update _allButtons array with the new order
        _allButtons = activeButtons.ToArray();
    }

    
    /// <summary>
    /// Highlights the button at the given index, ensuring correct prioritization.
    /// </summary>
    /// <param name="index">Index of the button to select.</param>
    private void SelectButton(int index)
    {
        if (_allButtons == null || _allButtons.Length == 0) return;

        // Clamp index to ensure it's within valid bounds
        index = Mathf.Clamp(index, 0, _allButtons.Length - 1);

        // Highlight the selected button
        _allButtons[index].Select();

        // Update the last selected button
        _lastSelectedButton = _allButtons[index].gameObject;
    }

    private void UpdateRunButtons()
    {
        // Enable only the completed runs
        runButton1.interactable = runCharacterSelections.ContainsKey(0);
        runButton2.interactable = runCharacterSelections.ContainsKey(1);
        runButton3.interactable = runCharacterSelections.ContainsKey(2);

        // Update text to show selected character for each run
        UpdateRunButtonText(runButton1, 0);
        UpdateRunButtonText(runButton2, 1);
        UpdateRunButtonText(runButton3, 2);
    }


    private void UpdateRunButtonText(Button runButton, int runIndex)
    {
        TextMeshProUGUI buttonText = runButton.GetComponentInChildren<TextMeshProUGUI>();
        if (runCharacterSelections.ContainsKey(runIndex))
        {
            buttonText.text = "Run " + (runIndex + 1) + "\n(" + runCharacterSelections[runIndex] + ")";
        }
        else
        {
            buttonText.text = "Run " + (runIndex + 1);
        }
    }

    private void UpdateCharacterButtons()
    {
        if (isTutorial)
        {
            // In tutorial mode, restrict selection to specific order
            plantCharacterButton.interactable = (_tutorialStep == 0);
            stoneCharacterButton.interactable = (_tutorialStep == 1);
            fireCharacterButton.interactable = (_tutorialStep == 2);
        }
        else if (isWaterTutorial)
        {
            // In tutorial mode, restrict selection to specific order
            stoneCharacterButton.interactable = (_tutorialStep == 0);
            plantCharacterButton.interactable = (_tutorialStep == 1);
            fireCharacterButton.interactable = (_tutorialStep == 2);
        } else 
        {
            plantCharacterButton.interactable = true;
            stoneCharacterButton.interactable = true;
            fireCharacterButton.interactable = true;

            foreach (var selection in runCharacterSelections)
            {
                if (selection.Key < _currentRun) // Only lock characters from previous runs
                {
                    if (selection.Value == "Plant") plantCharacterButton.interactable = false;
                    if (selection.Value == "Stone") stoneCharacterButton.interactable = false;
                    if (selection.Value == "Fire") fireCharacterButton.interactable = false;
                }
            }
        }

    }

    private void OnRunButtonPressed(int runIndex)
    {
        _projectEvents.DeactivateCurrentCharacter?.Invoke();
        _canClose = false;
        
        if (_currentRun == runIndex)
        {
            Debug.Log("Load Run: " + runIndex);
            _projectEvents.LoadRequested(runIndex); // Trigger loading the run state
        }
        else
        {
            if (isTutorial || isWaterTutorial)
            {
                // Reset the tutorial step to align with the selected run
                _tutorialStep = runIndex;

                Debug.Log($"Tutorial step reset to {_tutorialStep} for Run {runIndex}");
            }
            
            ResetToRun(runIndex);
        }
    }

    private void OnCharacterButtonPressed(string character)
    {
        #region Tutorial
        if (isTutorial)
        {
            // Check if the selected character matches the tutorial step
            if ((_tutorialStep == 0 && character != "Plant") ||
                (_tutorialStep == 1 && character != "Stone") ||
                (_tutorialStep == 2 && character != "Fire"))
            {
                Debug.LogWarning("Invalid selection in tutorial mode!");
                return;
            }
        }

        if (isWaterTutorial)
        {
            // Check if the selected character matches the tutorial step
            if ((_tutorialStep == 0 && character != "Stone") ||
                (_tutorialStep == 1 && character != "Plant") ||
                (_tutorialStep == 2 && character != "Fire"))
            {
                Debug.LogWarning("Invalid selection in tutorial mode!");
                return;
            }
        }
        #endregion
        
        Debug.Log(character + " character selected for Run " + _currentRun);

        // Save the character selection for the current run
        runCharacterSelections[_currentRun] = character;

        // Trigger event for character selection
        CharacterType characterType = character switch
        {
            "Plant" => CharacterType.Plant,
            "Stone" => CharacterType.Stone,
            "Fire" => CharacterType.Fire,
            _ => throw new ArgumentException("Invalid character type")
        };

        _projectEvents.Character_RequestCharacterSwitch?.Invoke(characterType);
        _projectEvents.LoadRequested(_currentRun);
        
        _projectEvents.UIChangeRunAndCharacterInformation?.Invoke(_currentRun, characterType);

        _canClose = true;
        CloseScreen();
    }

    private void ResetToRun(int runIndex)
    {
        _projectEvents.LoadRequested(runIndex);
        
        _currentRun = runIndex;
        
        //Call Level Manager to Reset Run to current Index.

        // Remove character selections for runs after the selected run
        List<int> keysToRemove = new List<int>();
        foreach (var key in runCharacterSelections.Keys)
        {
            if (key >= runIndex)
            {
                keysToRemove.Add(key);
            }
        }
        foreach (var key in keysToRemove)
        {
            runCharacterSelections.Remove(key);
        }
        
        _projectEvents.LoadRequested(runIndex);

        // Update UI
        InitializeUI();
    }
    
    private void CloseScreen()
    {
        if (_canClose)
        {
            selectionScreen.SetActive(false);
            _projectEvents.ScreenClosed?.Invoke();
            _projectEvents.UISetInGameState?.Invoke(InGameUI.UIState.Active);
        }

    }

    private void OpenScreen(int currentRun)
    {
        _projectEvents.UISetInGameState?.Invoke(InGameUI.UIState.Hidden);
        
        _currentRun = currentRun;

        if (isTutorial || isWaterTutorial) _tutorialStep = currentRun;
        
        
        selectionScreen.SetActive(true);

        // Update the UI based on current state
        InitializeUI();
    }

    private void FinishTutorial()
    {
        isTutorial = false;
        if(ResetIndicator is not null)
            ResetIndicator.SetActive(true);
    }
    
    
    private void HandleButtonReselection()
    {
        // Check if no button is currently selected
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Detect input from the D-Pad or joystick
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                // Always select the first Character Button if no button is selected
                SelectButton(0);
            }
        }
        else
        {
            // Update the last selected button
            _lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }

}
