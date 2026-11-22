using Events;
using SceneLoader;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class PauseMenu : MonoBehaviour
{
    [Inject] private ProjectEvents _projectEvents;

    [Header("PauseMenu GameObject")]
    [SerializeField] private GameObject _pauseMenuGameObject;
    [SerializeField] private GameObject _optionsMenuGameObject;

    [Header("Buttons")]
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _selectRunButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitLevelButton;

    [Header("ExitLevel Scene Group")]
    [SerializeField] private SceneSetups _exitSceneGroup;

    private bool _isPausing;
    private bool isInPauseMenu
    {
        get => _isPausing;
        set
        {
            _isPausing = value;

            Time.timeScale = _isPausing ? 0 : 1;
        }
    }

    private bool _canPause = true;
    private bool _canUnPause = true;

    private bool canPause
    { 
        get => _canPause;
        set
        {
            _canPause = value;
        }
    }

    #region Unity Methods


    private void Start()
    {
        _continueButton.onClick.AddListener(() => ContinueButtonClick());
        _selectRunButton.onClick.AddListener(() => SelectRunButtonClick());
        _optionsButton.onClick.AddListener(() => OptionsButtonClick());
        _quitLevelButton.onClick.AddListener(() => QuitLevelButtonClick());

    }

    private void OnEnable()
    {
        InputSystem.actions["PauseMenu"].performed += ToggleMenu;

        _projectEvents.OpenLevelEndScreen += () => { canPause = false; };
        _projectEvents.OpenSelectionScreen += (x) => { 
            canPause = false;
            isInPauseMenu = false;
        };
        _projectEvents.ScreenClosed += () => { 
            canPause = true; };

    }


    private void OnDisable()
    {
        InputSystem.actions["PauseMenu"].performed -= ToggleMenu;

        _projectEvents.OpenLevelEndScreen -= () => { canPause = false; };
        _projectEvents.OpenSelectionScreen -= (x) => {
            canPause = false;
            isInPauseMenu = false;
        };
        _projectEvents.ScreenClosed -= () => { canPause = true; };
    }

    private void OnDestroy()
    {
        _continueButton.onClick.RemoveListener(() => ContinueButtonClick());
        _selectRunButton.onClick.RemoveListener(() => SelectRunButtonClick());
        _optionsButton.onClick.RemoveListener(() => OptionsButtonClick());
        _quitLevelButton.onClick.RemoveListener(() => QuitLevelButtonClick());
    }

    #endregion

    private void ToggleMenu(InputAction.CallbackContext context)
    {
        if (!_canUnPause)
            return;

        if (isInPauseMenu || !canPause)
        {
            isInPauseMenu = false;
            _pauseMenuGameObject.SetActive(false);
            return;
        }

        isInPauseMenu = context.ReadValueAsButton();

        _pauseMenuGameObject.SetActive(true);

        _continueButton.Select();
    }

    private void ToggleMenu(bool isOn)
    {

        if (!canPause)
        {
            isInPauseMenu = false;
            _pauseMenuGameObject.SetActive(false);
            return;
        }

        _pauseMenuGameObject.SetActive(isOn);

        if (isInPauseMenu)
            _continueButton.Select();

        isInPauseMenu = isOn;
    }

    public void SelectFirstButton()
    {
        canPause = true;
        _canUnPause = true;
        _continueButton.Select();
    }

    #region Button Clicks
    private void ContinueButtonClick()
    {
        ToggleMenu(false);
    }

    private void SelectRunButtonClick()
    {
        ToggleMenu(false);
        _projectEvents.OpenCurrentRunSelectionScreen?.Invoke();
        isInPauseMenu = false;
        canPause = false;
    }

    private void OptionsButtonClick()
    {
        _optionsMenuGameObject.SetActive(true);
        _pauseMenuGameObject.SetActive(false);
        canPause = false;
        _canUnPause = false;
    }

    private void QuitLevelButtonClick()
    {
        _projectEvents.LoadToLevelByScene?.Invoke(_exitSceneGroup);
        ToggleMenu(false);
    }
    #endregion

}
