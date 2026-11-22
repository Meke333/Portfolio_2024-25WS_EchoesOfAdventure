using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using Interface;
using SceneLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;


public class LevelSelectionScreen : MonoBehaviour
{
    [Inject] private ISceneLoader _sceneLoader;
    [Inject] private ProjectEvents _projectEvents; // Inject your project events

    public GameObject MainMenu;
    
    [Header("Level Objects")]
    public List<GameObject> levelObjects;

    [Header("Buttons")]
    public List<Button> allButtons;

    public Button BackButton;

    [Header("Scene Setups")]
    public List<SceneSetups> sceneSetups;

    [Header("Settings")]
    public float scrollAnimationDuration = 0.3f;
    public float scrollAmount = 360f;
    
    [Header("LevelIndicator")]
    public GameObject LevelIndicator;
    private int _levelIndicatorIndex = 1;
    private int levelIndicatorIndex
    {
        get => _levelIndicatorIndex;
        set
        {
            _levelIndicatorIndex = value;

            SelectButton(levelIndicatorIndex);
        }
    }

    private int currentIndex = 0;
    private bool isScrolling = false;

    private bool isAtTop = false;
    private bool isAtBottom = false;
    
    
    private void OnEnable()
    {
        for (int i = 0; i < allButtons.Count; i++)
        {
            allButtons[i].interactable = true;
        }

        // Set up input listeners
        InputSystem.actions["CharacterSelectionUp"].started += OnScrollUp;
        InputSystem.actions["CharacterSelectionDown"].started += OnScrollDown;
        InputSystem.actions["CharacterSelectionLeft"].started += GoToBackButton;
        InputSystem.actions["CharacterSelectionRight"].started += GoToLevelSelect;
        InputSystem.actions["UIBack"].started += CallBackButton;
        UpdateTopBottomFlags();

        levelIndicatorIndex = 0;
    }
    

    private void OnDisable()
    {
        // Clean up input listeners
        InputSystem.actions["CharacterSelectionUp"].started -= OnScrollUp;
        InputSystem.actions["CharacterSelectionDown"].started -= OnScrollDown;
        InputSystem.actions["CharacterSelectionLeft"].started -= GoToBackButton;
        InputSystem.actions["CharacterSelectionRight"].started -= GoToLevelSelect;
        InputSystem.actions["UIBack"].started -= CallBackButton;
    }

    private void Awake()
    {
        AssignButtonCallbacks();
    }
    
    /// <summary>
    /// Highlights the button at the given index.
    /// </summary>
    /// <param name="index">Index of the button to select.</param>
    private void SelectButton(int index)
    {
        // Highlight the selected button
        allButtons[index].Select();
    }


    /// <summary>
    /// Assigns callbacks to buttons using serialized fields.
    /// </summary>
    private void AssignButtonCallbacks()
    {
        
        BackButton.onClick.AddListener(BackToMainMenu);
        for (int i = 0; i < allButtons.Count; i++)
        {
            int index = i; // Capture the loop variable
            allButtons[index].onClick.AddListener(() => StartLevel(index));
        }
    }

    private void StartLevel(int i)
    {
        for (int x = 0; x < allButtons.Count; x++)
        {
            allButtons[x].interactable = false;
        }

        SceneSetups compare = _sceneLoader.GetCurrentSceneSetup();
        if (compare == sceneSetups[i])
        {
            _projectEvents.ResetCurrentRun?.Invoke(0);
            _projectEvents.ResetGoal?.Invoke();
            _projectEvents.OpenSelectionScreen?.Invoke(0);
            return;
        }

        _projectEvents.LoadToLevelByScene?.Invoke(sceneSetups[i]);
    }

    private void BackToMainMenu()
    {
        MainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
    
    private void CallBackButton(InputAction.CallbackContext obj)
    {
        BackToMainMenu();
    }
    

    #region ScrollingLogic
    private void OnScrollUp(InputAction.CallbackContext context)
    {
        if (!isScrolling)
        {
            if (isAtBottom && levelIndicatorIndex <= 0)
            {
                MoveLevelIndicator(LevelIndicator.GetComponent<RectTransform>(), scrollAmount);
                levelIndicatorIndex++;   
            } else if (isAtTop && levelIndicatorIndex < levelObjects.Count - 1)
            {
                MoveLevelIndicator(LevelIndicator.GetComponent<RectTransform>(), scrollAmount);
                levelIndicatorIndex++;
            } else if (!isAtTop)
            {
                StartCoroutine(ScrollToIndex(currentIndex + 1));
            }
            else
            {
                UpdateTopBottomFlags();
            }
        }

        //SelectButton(levelIndicatorIndex);
    }

    private void OnScrollDown(InputAction.CallbackContext context)
    {

        if (!isScrolling)
        {
            if (isAtTop && levelIndicatorIndex == levelObjects.Count - 1)
            {
                MoveLevelIndicator(LevelIndicator.GetComponent<RectTransform>(), -scrollAmount);
                levelIndicatorIndex--;
            } else if (isAtBottom && levelIndicatorIndex > 0)
            {
                MoveLevelIndicator(LevelIndicator.GetComponent<RectTransform>(), -scrollAmount);
                levelIndicatorIndex--;
            } else if (!isAtBottom)
            {
                StartCoroutine(ScrollToIndex(currentIndex - 1));
            }
            else
            {
                UpdateTopBottomFlags();
            }

        }

        //SelectButton(levelIndicatorIndex);
    }
    private void GoToLevelSelect(InputAction.CallbackContext context)
    {
        SelectButton(levelIndicatorIndex);
    }

    private void GoToBackButton(InputAction.CallbackContext context)
    {
        BackButton.Select();
    }

    private IEnumerator ScrollToIndex(int newIndex)
    {
        isScrolling = true;

        // Cache initial positions
        Vector3[] initialPositions = new Vector3[levelObjects.Count];
        Vector3[] targetPositions = new Vector3[levelObjects.Count];

        for (int i = 0; i < levelObjects.Count; i++)
        {
            RectTransform rectTransform = levelObjects[i].GetComponent<RectTransform>();
            initialPositions[i] = rectTransform.anchoredPosition;
            targetPositions[i] = initialPositions[i] + new Vector3(0, -scrollAmount * (newIndex - currentIndex), 0);
        }

        float elapsedTime = 0;
        while (elapsedTime < scrollAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scrollAnimationDuration;

            for (int i = 0; i < levelObjects.Count; i++)
            {
                RectTransform rectTransform = levelObjects[i].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = Vector3.Lerp(initialPositions[i], targetPositions[i], t);
            }

            yield return null;
        }

        currentIndex = newIndex;
        levelIndicatorIndex = newIndex + 1;
        UpdateTopBottomFlags();
        isScrolling = false;
    }

    private void UpdateTopBottomFlags()
    {
        isAtBottom = currentIndex <= 0;
        isAtTop = currentIndex + 3 >= levelObjects.Count;
    }
    
    private void MoveLevelIndicator(RectTransform levelIndicator, float yOffset)
    {
        StartCoroutine(MoveIndicatorRoutine(levelIndicator, yOffset));
    }

    private IEnumerator MoveIndicatorRoutine(RectTransform levelIndicator, float yOffset)
    {
        isScrolling = true;

        Vector3 initialPosition = levelIndicator.anchoredPosition;
        Vector3 targetPosition = initialPosition + new Vector3(0, yOffset, 0);

        float elapsedTime = 0;
        while (elapsedTime < scrollAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scrollAnimationDuration;

            levelIndicator.anchoredPosition = Vector3.Lerp(initialPosition, targetPosition, t);

            yield return null;
        }

        isScrolling = false;
    }
    #endregion
    
}
