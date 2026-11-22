using System;
using System.Collections.Generic;
using Events;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public enum SelectionState
{
    CharacterSelection,
    RunSelection
}

public class CharacterRunSelectionUI : MonoBehaviour
{
    // Enum to manage selection states
    private SelectionState _currentSelectionState = SelectionState.CharacterSelection;

    [Header("UI Animator")]
    [SerializeField] private Animator _animator;

    [Header("UI Panels")]
    public RectTransform runSelectionPanel;       // The panel for Run Selection
    public RectTransform characterSelectionPanel; // The panel for Character Selection

    [SerializeField] private GameObject characterSelectionObjects;
    [SerializeField] private GameObject runSelectionObjects;
   

    [Header("Transition Settings")]
    [Tooltip("How many seconds the transition should take.")]
    public float transitionSpeed = 0.5f;          

    [Header("Mouse Region Thresholds")]
    [SerializeField] private float topRegionThreshold = 0.8f;    
    [SerializeField] private float bottomRegionThreshold = 0.2f; 
    

    private bool _isInRunSelection; // Tracks if the player is currently in the top region
    
    private bool _hasSwitchedOnMouseEnter = false; // Prevents immediate switching on Start

    #region SelectionLogicVariables

    [Inject] private ProjectEvents _projectEvents;
    
    [Header("Run Buttons")]
    public Button runButton1;
    public Button runButton2;
    public Button runButton3;
    private Button[] _runButtons;

    [Header("Character Screen Run Visualization")]
    public Image characterRun1Visualization;
    public Image characterRun2Visualization;
    public Image characterRun3Visualization;
    private Image[] _runVisuals;

    [SerializeField] private Sprite plantSprite;
    [SerializeField] private Sprite stoneSprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite activeRunSprite;

    [Header("Run Indicator")]
    public RectTransform runIndicator;
    public float[] runIndicatorPositions = { -420f, 0f, 420f };
    public TextMeshProUGUI runIndicatorText;

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

    private bool _isFinishedTutorialTriggered = false;

    public GameObject ResetIndicator;

    private int _tutorialStep = 0;

    private bool _canClose = true;
    
    private GameObject _lastSelectedButton; // Tracks the last selected button
    private List<Button> _allButtons; // Array to hold all selectable buttons
    
    private Dictionary<int, string> runCharacterSelections = new Dictionary<int, string>(); // Tracks character selections per run

    #endregion

    private bool _isOpen;

    void Start()
    {

        _runButtons = new[] { runButton1, runButton2, runButton3 };
        _runVisuals = new[] { characterRun1Visualization, characterRun2Visualization, characterRun3Visualization };
        
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
        _projectEvents.OpenCurrentRunSelectionScreen += OpenToRunScreen;
        _projectEvents.ResetCurrentRun += ResetToRun;
        _projectEvents.FinishedTutorial += FinishTutorial;
        _projectEvents.CloseSelectionScreen += CloseScreen;
        
        // Reset to Character Selection state when the object is enabled
        SetSelectionState(SelectionState.CharacterSelection, immediate: true);
        _hasSwitchedOnMouseEnter = false; // Reset the mouse switch flag
        
        InputSystem.actions["CharacterSelectionClick"].started += MouseClick;
        InputSystem.actions["CharacterSelectionUp"].started += OpenRunScreen;
        InputSystem.actions["CharacterSelectionDown"].started += OpenCharacterScreen;
    }
    
    private void OnDisable()
    {
        _projectEvents.OpenSelectionScreen -= OpenScreen;
        _projectEvents.OpenCurrentRunSelectionScreen -= OpenToRunScreen;
        _projectEvents.ResetCurrentRun -= ResetToRun;
        _projectEvents.FinishedTutorial -= FinishTutorial;
        _projectEvents.CloseSelectionScreen -= CloseScreen;
        InputSystem.actions["CharacterSelectionClick"].started -= MouseClick;
        InputSystem.actions["CharacterSelectionUp"].started -= OpenRunScreen;
        InputSystem.actions["CharacterSelectionDown"].started -= OpenCharacterScreen;
    }

    private void Update()
    {
        HandleButtonReselection();
    }

    #region Character Selection Logic

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
        runIndicatorText.text = "Run " + (_currentRun + 1);

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

        // Update _allButtons with the new order
        _allButtons = activeButtons;
    }

    
    /// <summary>
    /// Highlights the button at the given index, ensuring correct prioritization.
    /// </summary>
    /// <param name="index">Index of the button to select.</param>
    private void SelectButton(int index)
    {
        if (_allButtons == null || _allButtons.Count == 0) return;

        // Clamp index to ensure it's within valid bounds
        index = Mathf.Clamp(index, 0, _allButtons.Count - 1);

        // Highlight the selected button
        _allButtons[index].Select();

        // Update the last selected button
        _lastSelectedButton = _allButtons[index].gameObject;
    }

    private void UpdateRunButtons()
    {
        // Enable only the completed runs
        runButton1.interactable = runCharacterSelections.ContainsKey(0) || _currentRun == 0;
        runButton2.interactable = runCharacterSelections.ContainsKey(1) || _currentRun == 1;
        runButton3.interactable = runCharacterSelections.ContainsKey(2) || _currentRun == 2;

        // Update text to show selected character for each run
        UpdateRunButtonText(runButton1, 0, characterRun1Visualization);
        UpdateRunButtonText(runButton2, 1, characterRun2Visualization);
        UpdateRunButtonText(runButton3, 2, characterRun3Visualization);
    }


    private void UpdateRunButtonText(Button runButton, int runIndex, Image runVisualization)
    {
        if (runCharacterSelections.ContainsKey(runIndex) && (_canClose || runIndex != _currentRun))
        {
            // Get the selected character for this run
            string selectedCharacter = runCharacterSelections[runIndex];

            // Update the button 
            runButton.GetComponent<Image>().sprite = selectedCharacter switch
            {
                "Plant" => plantSprite,
                "Stone" => stoneSprite,
                "Fire"  => fireSprite,
                _       => defaultSprite // Fallback if no match
            };

            runVisualization.sprite = selectedCharacter switch
            {
                "Plant" => plantSprite,
                "Stone" => stoneSprite,
                "Fire"  => fireSprite,
                _       => defaultSprite // Fallback if no match
            };
        }
        else
        {
            // If it's the current run and no character is selected, use an active indicator sprite
            if (runIndex == _currentRun)
            {
                runButton.GetComponent<Image>().sprite = activeRunSprite;
                runVisualization.sprite = activeRunSprite;
            }
            else
            {
                // Reset to default sprite for other runs with no selection
                runButton.GetComponent<Image>().sprite = defaultSprite;
                runVisualization.sprite = defaultSprite;
            }
        }
    }

    private void ResetRunButtonText(Button runButton, int runIndex, Image runVisualization)
    {
        if (runIndex == _currentRun)
        {
            runButton.GetComponent<Image>().sprite = activeRunSprite;
            runVisualization.sprite = activeRunSprite;
        }
        else
        {
            // Reset to default sprite for other runs with no selection
            runButton.GetComponent<Image>().sprite = defaultSprite;
            runVisualization.sprite = defaultSprite;
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
        _projectEvents.Input_TogglePlayerInput?.Invoke(false);

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
        
        SetSelectionState(SelectionState.CharacterSelection);

        switch(runIndex)
        {
            case 0:
                UpdateRunButtonText(runButton1, runIndex, characterRun1Visualization);
                break;
            case 1:
                UpdateRunButtonText(runButton2, runIndex, characterRun2Visualization);
                break;
            case 2:
                UpdateRunButtonText(runButton3, runIndex, characterRun3Visualization);
                break;
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

        _projectEvents.Input_TogglePlayerInput?.Invoke(true);
        Time.timeScale = 1;
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
            _isOpen = false;

            selectionScreen.SetActive(false);
            _projectEvents.ScreenClosed?.Invoke();
            _projectEvents.UISetInGameState?.Invoke(InGameUI.UIState.Active);

            _projectEvents.Input_TogglePlayerInput?.Invoke(true);


            Time.timeScale = 1;
        }

    }

    private void OpenScreen(int currentRun)
    {
        _isOpen = true;
        _projectEvents.Input_TogglePlayerInput?.Invoke(false);
        _projectEvents.UISetInGameState?.Invoke(InGameUI.UIState.Hidden);

        _currentRun = currentRun;

        if (isTutorial || isWaterTutorial)
        {
            _tutorialStep = currentRun;
        }

        selectionScreen.SetActive(true);
        if (_isFinishedTutorialTriggered)
        {
            _isFinishedTutorialTriggered = false;
            OpenToRunScreen();

            UpdateRunIndicator();
            UpdateRunButtons();
            UpdateCharacterButtons();
            return;
        }

        // Reset to Character Selection state when the object is enabled
        SetSelectionState(SelectionState.CharacterSelection, immediate: true);

        // Update the UI based on current state
        InitializeUI();
    }

    private void OpenToRunScreen()
    {
        _isOpen = true;
        _projectEvents.Input_TogglePlayerInput?.Invoke(false);
        _projectEvents.UISetInGameState?.Invoke(InGameUI.UIState.Hidden);

        if (isTutorial || isWaterTutorial) _tutorialStep = _currentRun;

        // Reset to Character Selection state when the object is 
        selectionScreen.SetActive(true);
        SetSelectionState(SelectionState.RunSelection, immediate: true);
        SelectFirstTopButton();


        // Update the UI based on current state
        // InitializeUI();
    }



    private void FinishTutorial()
    {
        _isFinishedTutorialTriggered = true;

        isTutorial = false;
        if(ResetIndicator is not null)
            ResetIndicator.SetActive(true);
    }
    
    
    private void HandleButtonReselection()
    {
        if (!_isOpen)
            return;


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

    #endregion

    #region Expanding Areas

    
    private void OpenCharacterScreen(InputAction.CallbackContext obj)
    {
        if (!_isOpen)
            return;

        SetSelectionState(SelectionState.CharacterSelection);
    }

    private void OpenRunScreen(InputAction.CallbackContext obj)
    {
        if (!_isOpen)
            return;

        SetSelectionState(SelectionState.RunSelection);
    }

    
    private void SetSelectionState(SelectionState newState, bool immediate = false)
    {
        if (!_isOpen)
            return;


        if (_currentSelectionState == newState) return;

        _currentSelectionState = newState;

        // Control which screen is visible based on the state
        switch (newState)
        {
            case SelectionState.CharacterSelection:
                characterSelectionObjects.SetActive(true);
                runSelectionObjects.SetActive(false);
                //Expand(characterSelectionPanel, topRegionThreshold, immediate);
                _animator.SetBool(Constants.AnimatorParameters.CharacterRunSelectionUI_IsOnRunSelection, false);

                // Automatically select the first button in the bottom area
                SelectFirstBottomButton();
                break;

            case SelectionState.RunSelection:
                //Expand(characterSelectionPanel, bottomRegionThreshold, immediate);
                _animator.SetBool(Constants.AnimatorParameters.CharacterRunSelectionUI_IsOnRunSelection, true);

                characterSelectionObjects.SetActive(false);
                runSelectionObjects.SetActive(true);
                // Automatically select the first button in the top area
                SelectFirstTopButton();
                break;
        }
    }
    
    private void MouseClick(InputAction.CallbackContext obj)
    {
        if (!_isOpen)
            return;

        CheckMouseRegion();
    }
    
    private void CheckMouseRegion()
    {
        if (!_isOpen)
            return;

        // Get the mouse Y position as a percentage of screen height
        float mouseYPercentage = Input.mousePosition.y / Screen.height;

        // Check if the mouse is in the top region
        if (_currentSelectionState != SelectionState.RunSelection && mouseYPercentage >= topRegionThreshold)
        {
            Debug.Log("Mouse clicked in Top Region");
            SetSelectionState(SelectionState.RunSelection);
        }
        // Check if the mouse is in the bottom region
        else if (_currentSelectionState != SelectionState.CharacterSelection && mouseYPercentage <= bottomRegionThreshold)
        {
            Debug.Log("Mouse clicked in Bottom Region");
            SetSelectionState(SelectionState.CharacterSelection);
        }
    }
    
     /// <summary>
    /// Smoothly transitions the given panel from its current height
    /// to (targetFraction * Screen.height) over 'transitionSpeed' seconds.
    /// </summary>
    /// <param name="panel">The RectTransform of the panel to animate.</param>
    /// <param name="targetFraction">The fraction (0..1) of Screen.height to set as the panel's new height.</param>
     private void Expand(RectTransform panel, float targetFraction, bool immediate)
     {
         RectTransform parentRect = panel.parent as RectTransform;

         if (!parentRect)
         {
             Debug.LogWarning("No parent RectTransform found. Falling back to Screen.height.");
             StartCoroutine(SmoothHeightChange(panel, Screen.height * targetFraction, immediate ? 0 : transitionSpeed));
             return;
         }

         float targetHeight = parentRect.rect.height * targetFraction;

         StopAllCoroutines(); // Stop any ongoing animations
         StartCoroutine(SmoothHeightChange(panel, targetHeight, immediate ? 0 : transitionSpeed));
     }

     private System.Collections.IEnumerator SmoothHeightChange(RectTransform panel, float newHeight, float duration)
     {
         float startHeight = panel.rect.height;
         float elapsedTime = 0f;

         while (elapsedTime < duration)
         {
             elapsedTime += Time.deltaTime;

             float t = Mathf.Clamp01(elapsedTime / duration);
             float currentHeight = Mathf.Lerp(startHeight, newHeight, t);

             Vector2 sizeDelta = panel.sizeDelta;
             sizeDelta.y = currentHeight;
             panel.sizeDelta = sizeDelta;

             yield return null;
         }

         Vector2 finalSize = panel.sizeDelta;
         finalSize.y = newHeight;
         panel.sizeDelta = finalSize;
     }
     
// Helper method to select the first button in the top area
     private void SelectFirstTopButton()
     {
        if (!_isOpen)
            return;


        switch (_currentRun)
        {
            case 0:
                if (runButton1 != null && runButton1.interactable)
                {
                    runButton1.Select();
                }
                break;
            case 1:
                if (runButton2 != null && runButton2.interactable)
                {
                    runButton2.Select();
                }
                break;
            case 2:
                if (runButton3 != null && runButton3.interactable)
                {
                    runButton3.Select();
                }
                break;

            default:
                break;
        }
     }

// Helper method to select the first button in the bottom area
     private void SelectFirstBottomButton()
     {
        if (!_isOpen)
            return;


        if (plantCharacterButton != null && plantCharacterButton.interactable)
         {
             plantCharacterButton.Select();
         } else if (stoneCharacterButton != null && stoneCharacterButton.interactable)
         {
             stoneCharacterButton.Select();
         }
         else
         {
             fireCharacterButton.Select();
         }
     }
     
    

    #endregion
}
