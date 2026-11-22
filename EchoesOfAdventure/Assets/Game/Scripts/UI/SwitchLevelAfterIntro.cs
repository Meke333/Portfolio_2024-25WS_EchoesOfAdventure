using System;
using Events;
using SceneLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using Zenject;

public class SwitchLevelAfterIntro : MonoBehaviour
{
    
    [Inject]private ProjectEvents _projectEvents;
    [SerializeField] private PlayableDirector playableDirector; // Reference to the PlayableDirector
    [SerializeField] private SceneSetups _nextScene;

    private bool hasTriggered = false; // To ensure the event is triggered only once

    private void OnEnable()
    {
        InputSystem.actions["SkipCutscene"].performed += SkipTimeline;
    }
    
    private void OnDisable()
    {
        InputSystem.actions["SkipCutscene"].performed -= SkipTimeline;
    }

    private void Start()
    {
        if (playableDirector == null)
        {
            Debug.LogError("PlayableDirector is not assigned in the inspector!");
            return;
        }
        
        _projectEvents.LevelStart?.Invoke();

        // Subscribe to the PlayableDirector's stopped event
        playableDirector.stopped += OnPlayableDirectorStopped;
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        // Check if the stopped director is the assigned one
        if (director == playableDirector && !hasTriggered)
        {
            Debug.Log("Timeline finished!");

            // Trigger the event
           _projectEvents.LoadToLevelByScene?.Invoke(_nextScene);

            // Ensure the event is only triggered once per playback
            hasTriggered = true;
        }
    }
    
    private void SkipTimeline(InputAction.CallbackContext obj)
    {
        if (playableDirector != null && playableDirector.state == PlayState.Playing)
        {
            playableDirector.Stop(); // Stop the timeline
            OnPlayableDirectorStopped(playableDirector); // Manually call the event
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnPlayableDirectorStopped;
        }
    }
}