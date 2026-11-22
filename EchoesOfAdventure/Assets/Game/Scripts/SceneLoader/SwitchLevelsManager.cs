using System;
using System.Collections;
using Events;
using Interface;
using SceneLoader;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

public class SwitchLevelsManager : MonoBehaviour
{
    private int _currentLevel = 0;

    [Inject] private ISceneLoader _iSceneLoader; // Inject the scene loader interface
    [Inject] private ProjectEvents _projectEvents; // Inject your project events
    
    [SerializeField] private PlayableDirector fadeOutTimeline;
    [SerializeField] private PlayableDirector fadeInTimeline;

    private void OnEnable()
    {
        _projectEvents.LoadNextLevel += LoadNextLevel;
        _projectEvents.LoadToLevelByIndex += LoadToLevel;
        _projectEvents.LoadToLevelByScene += StartFadeOutAndLoadScene;
        _projectEvents.LevelStart += FadeIn;
        
    }
    
    private void OnDisable()
    {
        _projectEvents.LoadNextLevel -= LoadNextLevel;
        _projectEvents.LoadToLevelByIndex -= LoadToLevel;
        _projectEvents.LoadToLevelByScene -= StartFadeOutAndLoadScene;
        _projectEvents.LevelStart -= FadeIn;
    }
    

    /// <summary>
    /// Load the next level in the sequence based on the SceneSetups enum.
    /// </summary>
    private void LoadNextLevel()
    {
        if (_currentLevel < Enum.GetValues(typeof(SceneSetups)).Length - 1)
        {
            FadeOut();
            _currentLevel++;
            Debug.Log($"Switching to next level: {_currentLevel} ({(SceneSetups)_currentLevel})");
            StartFadeOutAndLoadScene((SceneSetups)_currentLevel);
        }
        else
        {
            Debug.LogWarning("Already at the last level. Cannot load next level.");
        }
    }
    
    

    /// <summary>
    /// Load a specific level by its index.
    /// </summary>
    /// <param name="levelIndex">Index of the level to load.</param>
    private void LoadToLevel(int levelIndex)
    {
        if (Enum.IsDefined(typeof(SceneSetups), levelIndex))
        {
            FadeOut();
            _currentLevel = levelIndex;
            Debug.Log($"Switching to specific level: {levelIndex} ({(SceneSetups)_currentLevel})");
            StartFadeOutAndLoadScene((SceneSetups)levelIndex);
        }
        else
        {
            Debug.LogError($"Invalid level index: {levelIndex}. Cannot load level.");
        }
    }
    
    
    private void FadeIn()
    {
        fadeInTimeline.Play();
    }

    private void FadeOut()
    {
        fadeOutTimeline.Play();
    }
    
    // This method is subscribed to the event instead of LoadSceneSetup directly
    private void StartFadeOutAndLoadScene(SceneSetups setup)
    {
        // We call a coroutine so we can do async steps (fade out, then load)
        StartCoroutine(FadeOutAndLoadCoroutine(setup));
    }

    private IEnumerator FadeOutAndLoadCoroutine(SceneSetups setup)
    {
        // Optionally reset timeline
        fadeOutTimeline.time = 0f;

        // Start fade out
        fadeOutTimeline.Play();

        // Wait until the timeline finishes playing
        yield return new WaitUntil(() => fadeOutTimeline.state != PlayState.Playing);

        // Now load the scene after fade-out completes
        Debug.Log($"Now loading scene setup: {setup}");
        _iSceneLoader.LoadSceneSetUp(setup);
    }

    /*
    /// <summary>
    /// Load a scene setup using the ISceneLoader.
    /// </summary>
    /// <param name="setup">The SceneSetups enum value representing the scene setup to load.</param>
    private void LoadSceneSetup(SceneSetups setup)
    {
        Debug.Log($"Loading scene setup: {setup}");
        FadeOut();
        _iSceneLoader.LoadSceneSetUp(setup); // Use the provided method in ISceneLoader
    }
    
    */
}
