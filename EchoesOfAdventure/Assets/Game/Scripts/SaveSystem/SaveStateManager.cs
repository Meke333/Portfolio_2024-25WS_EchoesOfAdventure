using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Echo;
using Events;
using Interactable;
using Interface;
using UnityEngine;
using Zenject;

/// <summary>
/// Manages saving and loading the state of interactable and echo objects in the game.
/// </summary>
public class SaveStateManager : MonoBehaviour
{
    [Inject]private ProjectEvents _projectEvents;
    
    // GameObjects containing interactables and echoes.
    [Header("Objects To Save")]
    [SerializeField] private GameObject _interactables;
    [SerializeField] private GameObject _echoes;
    
    // Lists to store save states for different iterations (3 iterations supported).
    private const int MaxIterations = 3;
    private List<SaveState>[] _interactablesSaveStates = new List<SaveState>[MaxIterations];
    private List<SaveState>[] _echoesSaveStates = new List<SaveState>[MaxIterations];

    // Cached lists of GameObjects for interactables and echoes.
    private GameObject[] _interactableGameObjectList;
    private GameObject[] _echoesGameObjectList;
    

    [SerializeField, ReadOnly(true)] private int lastLoadedLevel = 0;
    

    private void OnEnable()
    {
        if (_projectEvents != null)
        {
            _projectEvents.SaveRequested += SaveStateToCurrentIteration;
            _projectEvents.LoadRequested += LoadState;
            _projectEvents.ActivateSaveSystem += GetSaveObjects;
            _projectEvents.PlayerFinishedLevel += LevelEnd;
        }
        
        StartCoroutine(WaitForPoolInitialization());
    }

    private void OnDisable()
    {
        if (_projectEvents != null)
        {
            _projectEvents.SaveRequested -= SaveStateToCurrentIteration;
            _projectEvents.LoadRequested -= LoadState;
            _projectEvents.ActivateSaveSystem -= GetSaveObjects;
            _projectEvents.PlayerFinishedLevel -= LevelEnd;
        }
    }

    private void GetSaveObjects()
    {
        _interactableGameObjectList = GetInteractableGameObjects(_interactables);
        _echoesGameObjectList = GetEchoesGameObjects(_echoes);
        _projectEvents.SavePossible?.Invoke();
    }
    
    private IEnumerator WaitForPoolInitialization()
    {
        float timeout = 5f; // Maximum time to wait
        float interval = 0.5f; // Check every 0.5 seconds
        float elapsedTime = 0f;

        while (elapsedTime < timeout)
        {
            GetSaveObjects(); // Update the cached lists
            yield return new WaitForSeconds(interval);
            elapsedTime += interval;

            // Exit early if all objects are found
            if (_interactableGameObjectList.Length > 1 && _echoesGameObjectList.Length > 1)
            {
                _projectEvents.LevelStart?.Invoke();
                break;
            }
        }
    }
    
    public void SaveStateToCurrentIteration(int currentState)
    {
        Debug.LogWarning("SaveRequested");
        _interactablesSaveStates[currentState] = SaveInteractableState();
        _echoesSaveStates[currentState] = SaveEchoState();
    }
    public void LoadState(int currentState)
    {
        lastLoadedLevel = currentState;

        LoadInteractables(lastLoadedLevel);
        LoadEchoes(lastLoadedLevel);
    }
    
    #region Load States

    /// <summary>
    /// Loads the state of all interactables from the specified iteration.
    /// </summary>
    /// <param name="state">The iteration index to load the state from.</param>
    private void LoadInteractables(int state)
    {
        // Ensure that the state index is valid
        if (state < 0 || state >= _interactablesSaveStates.Length) return;

        // Get the list of saved states for the given iteration
        List<SaveState> savedStates = _interactablesSaveStates[state];
        

        for (int i = 0; i < _interactableGameObjectList.Length; i++)
        {
            _interactableGameObjectList[i].GetComponent<InteractableBase>().State = savedStates[i].InteractableState;
            _interactableGameObjectList[i].transform.position = savedStates[i].Position;
            _interactableGameObjectList[i].transform.rotation = savedStates[i].Rotation;
            _interactableGameObjectList[i].SetActive(savedStates[i].IsActive);
            _interactableGameObjectList[i].GetComponent<InteractableBase>().SolveStateBehaviour();

        }

    }

    /// <summary>
    /// Loads the state of all echoes from the specified iteration.
    /// </summary>
    /// <param name="state">The iteration index to load the state from.</param>
    private void LoadEchoes(int state)
    {
        // Ensure that the state index is valid
        if (state < 0 || state >= _echoesSaveStates.Length) return;

        // Get the list of saved states for the given iteration
        List<SaveState> savedStates = _echoesSaveStates[state];

        for (int i = 0; i < _echoesGameObjectList.Length; i++)
        {
            _echoesGameObjectList[i].GetComponent<EchoBase>().State = savedStates[i].EchoState;
            _echoesGameObjectList[i].transform.position = savedStates[i].Position;
            _echoesGameObjectList[i].transform.rotation = savedStates[i].Rotation;
            _echoesGameObjectList[i].SetActive(savedStates[i].IsActive);
            _echoesGameObjectList[i].GetComponent<EchoBase>().SolveStateBehaviour();
            
        }
    }

    #endregion
    
    #region SaveStates
   
    /// <summary>
    /// Saves the current state of all interactable objects.
    /// </summary>
    /// <returns>A list of SaveState objects representing the current state of interactables.</returns>
    private List<SaveState> SaveInteractableState()
    {
        //Create a Interactable List of Save States
        List<SaveState> result = new List<SaveState>();
        
        //Fill out the Save States for the interactables
        for (int i = 0; i < _interactableGameObjectList.Length; i++)
        {
            InteractableBase test = _interactableGameObjectList[i].GetComponent<InteractableBase>();

            var tempState = new SaveState();
                tempState.InteractableState = _interactableGameObjectList[i].GetComponent<InteractableBase>().State;
                tempState.Position = _interactableGameObjectList[i].transform.position;
                tempState.Rotation = _interactableGameObjectList[i].transform.rotation;
                tempState.IsActive = _interactableGameObjectList[i].activeSelf;
                result.Add(tempState);
        }
        
        return result;

    }

    /// <summary>
    /// Saves the current state of all echo objects.
    /// </summary>
    /// <returns>A list of SaveState objects representing the current state of echoes.</returns>
    private List<SaveState> SaveEchoState()
    {
        //Create a Echo List of Save States
        List<SaveState> result = new List<SaveState>();
        
        //Fill out the Save States for the Echoes
        for (int i = 0; i < _echoesGameObjectList.Length; i++)
        {
            var tempState = new SaveState();
            tempState.EchoState = _echoesGameObjectList[i].GetComponent<EchoBase>().State;
            tempState.Position = _echoesGameObjectList[i].transform.position;
            tempState.Rotation = _echoesGameObjectList[i].transform.rotation;
            tempState.IsActive = _echoesGameObjectList[i].activeSelf;
            result.Add(tempState);
        }
        
        return result;
    }

    #endregion

    #region Level End Logic

    private void LevelEnd()
    {
        for (int i = 0; i < _interactableGameObjectList.Length; i++)
        {
            _interactableGameObjectList[i].SetActive(false);
        }
        
        for (int i = 0; i < _echoesGameObjectList.Length; i++)
        {
            _echoesGameObjectList[i].SetActive(false);
        }

        _projectEvents.ReadyToSwitchLevel?.Invoke();
    }
    

    #endregion
    
    #region Helper
   
    /// <summary>
    /// Gets all child GameObjects of a specified parent Transform.
    /// </summary>
    /// <param name="transform">The parent Transform.</param>
    /// <returns>An array of child GameObjects.</returns>
    public static GameObject[] GetChildGameObjects(Transform transform)
    {
        List<GameObject> result = new List<GameObject>();
        CollectChildGameObjects(transform, result);
        return result.ToArray();
    }

    /// <summary>
    /// Recursively collects child GameObjects from a specified Transform.
    /// </summary>
    /// <param name="transform">The parent Transform.</param>
    /// <param name="result">The list to store the collected GameObjects.</param>
    private static void CollectChildGameObjects(Transform transform, List<GameObject> result)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            result.Add(child.gameObject); // Add the current child to the list
            CollectChildGameObjects(child, result); // Recursively collect its children
        }
    }
    
    /// <summary>
    /// Gets all GameObjects with an IInteractable component from a specified parent.
    /// </summary>
    /// <param name="parent">The parent GameObject.</param>
    /// <returns>An array of GameObjects with IInteractable components.</returns>
    private GameObject[] GetInteractableGameObjects(GameObject parent)
    {
        List<GameObject> interactableObjects = new List<GameObject>();

        // Get all child game objects
        GameObject[] allChildObjects = GetChildGameObjects(parent.transform);

        foreach (var child in allChildObjects)
        {
            // Check if the object has an IInteractable component
            if (child.GetComponent<InteractableBase>() != null)
            {
                interactableObjects.Add(child);
            }
        }

        return interactableObjects.ToArray();
    }
    
    /// <summary>
    /// Gets all GameObjects with an EchoBase component from a specified parent.
    /// </summary>
    /// <param name="parent">The parent GameObject.</param>
    /// <returns>An array of GameObjects with EchoBase components.</returns>
    private GameObject[] GetEchoesGameObjects(GameObject parent)
    {
        List<GameObject> echoObjects = new List<GameObject>();

        // Get all child game objects
        GameObject[] allChildObjects = GetChildGameObjects(parent.transform);

        foreach (var child in allChildObjects)
        {
            // Check if the object has an IInteractable component
            if (child.GetComponent<EchoBase>() != null)
            {
                echoObjects.Add(child);
            }
        }

        return echoObjects.ToArray();
    }
    
    #endregion

}
