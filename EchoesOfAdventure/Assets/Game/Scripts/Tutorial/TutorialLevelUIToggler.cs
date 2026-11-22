using System;
using Events;
using UnityEngine;
using Zenject;

public class TutorialLevelUIToggler : MonoBehaviour
{
    [Inject] private ProjectEvents _projectEvents;

    [SerializeField] private bool shouldDisableWhenTutorialIsFinished; //Index 3 should hold information after Tutorial
    [SerializeField] private ToggleGameObjectList[] ToggleGameObjects = new  ToggleGameObjectList[3];
    
    private int _currentRun;
    private bool _tutorialFinished;

    #region Classes

    [Serializable]
    public class ToggleGameObjectList
    {
        public ToggleGameObjectElement[] List;
    }
    [Serializable]
    public class ToggleGameObjectElement
    {
        public GameObject GameObject;
        public bool IsActive;
    }

    #endregion
    
    
    #region UnityMethods

    private void OnEnable()
    {
        ToggleEvents(true);
    }

    private void OnDisable()
    {
        ToggleEvents(false);
    }

    #endregion

    private void ToggleEvents(bool shouldSubscribe)
    {
        if (shouldSubscribe)
        {
            _projectEvents.OpenSelectionScreen += ResetToRun;
            _projectEvents.LoadRequested += ResetToRun;
            _projectEvents.FinishedTutorial += FinishedTutorial;
        }
        else
        {
            _projectEvents.OpenSelectionScreen -= ResetToRun;
            _projectEvents.LoadRequested -= ResetToRun;
            _projectEvents.FinishedTutorial -= FinishedTutorial;
        }
    }

    private void FinishedTutorial()
    {
        if (!shouldDisableWhenTutorialIsFinished || _tutorialFinished)
            return;

        _tutorialFinished = true;

        if (3 >= ToggleGameObjects.Length)
            return;
            
        for (int y = 0; y < ToggleGameObjects[3].List.Length; y++)
        {
            ToggleGameObjects[3].List[y].GameObject.SetActive(ToggleGameObjects[3].List[y].IsActive);
        }
    }

    private void ResetToRun(int obj)
    {
        
        if (_tutorialFinished)
            return;
        
        _currentRun = obj;

        for (int y = 0; y < ToggleGameObjects[_currentRun].List.Length; y++)
        {
            ToggleGameObjects[_currentRun].List[y].GameObject.SetActive(ToggleGameObjects[_currentRun].List[y].IsActive);
        }

    }
}

