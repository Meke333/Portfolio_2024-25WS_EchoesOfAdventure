
using Events;
using FMOD.Studio;
using Sounds;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Interactable
{
    public class InteractableSound : MonoBehaviour
	{
		[Inject] private ProjectEvents _projectEvents;

		[Inject] private InteractableEvents _events;
		[Inject] private SoundManager _audioManager;
		[Inject] private FMODEventsConfiguration _FMODEventsConfiguration;

		//Toggleable SFXs
		private SoundInstanceHandler _woodenBoxDrag;

		private bool _isSoundOn;

        private void Start()
        {
			SpawnAllInstances();

                //_projectEvents.OpenSelectionScreen += (x) => ToggleSoundActivity(false);
                _projectEvents.Character_CharacterSelected += (x) => ToggleSoundActivity(true);

            _projectEvents.ResetCurrentRun += ResetRun;

            _events.Collectible_Collected += (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.Collectible_Collected); };

            _events.WoodenBox_Breaks += (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.WoodenBox_Break); };
            _events.WoodenBox_Lands += (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.WoodenBox_Fall); };
            _events.WoodenBox_Drag += (x) => { ResolveLoopSoundDemand(x, InteractableSoundType.WoodenBox_Drag); };

            _events.Fire_Burning += (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.Fire_Burning); };

        }

        private void OnDisable()
        {
            StopAll();
        }

        private void OnDestroy()
        {
            //_projectEvents.OpenSelectionScreen -= (x) => ToggleSoundActivity(false);
            _projectEvents.Character_CharacterSelected -= (x) => ToggleSoundActivity(true);

            _projectEvents.ResetCurrentRun -= ResetRun;

            _events.Collectible_Collected -= (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.Collectible_Collected); };

            _events.WoodenBox_Breaks -= (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.WoodenBox_Break); };
            _events.WoodenBox_Lands -= (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.WoodenBox_Fall); };
            _events.WoodenBox_Drag -= (x) => { ResolveLoopSoundDemand(x, InteractableSoundType.WoodenBox_Drag); };

            _events.Fire_Burning -= (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.Fire_Burning); };


            ToggleSoundActivity(false);
        }

        private void SpawnAllInstances()
        {
            _woodenBoxDrag = _audioManager.FactorySoundAudioHandler.CreateEventInstance(_FMODEventsConfiguration.GetEventReference(InteractableSoundType.WoodenBox_Drag));
        }
        private void ResetRun(int obj)
        {
            StopAll();
        }

        private void ToggleSoundActivity(bool isOn)
		{
            Debug.Log("Toggle _isSoundOn: " + _isSoundOn + "; isOn: " + isOn);

            _isSoundOn = isOn;
            return;

            if (_isSoundOn == isOn)
				return;

            Debug.Log("Toggle success");

            switch (isOn)
			{
				case true:
                    _events.Collectible_Collected += (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.Collectible_Collected); };

                    _events.WoodenBox_Breaks += (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.WoodenBox_Break); };
                    _events.WoodenBox_Lands += (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.WoodenBox_Fall); };
                    _events.WoodenBox_Drag += (x) => { ResolveLoopSoundDemand(x, InteractableSoundType.WoodenBox_Drag); };

                    _events.Fire_Burning += (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.Fire_Burning); };

                    break;
				case false:
                    _events.Collectible_Collected -= (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.Collectible_Collected); };

                    _events.WoodenBox_Breaks -= (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.WoodenBox_Break); };
                    _events.WoodenBox_Lands -= (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.WoodenBox_Fall); };
                    _events.WoodenBox_Drag -= (x) => { ResolveLoopSoundDemand(x, InteractableSoundType.WoodenBox_Drag); };

                    _events.Fire_Burning -= (x) => { ResolveSingleSoundDemand(x, InteractableSoundType.Fire_Burning); };

                    break;
			}

        }

        public void ResolveSingleSoundDemand(Vector3 pos, InteractableSoundType type)
		{
            if (!_isSoundOn)
                return;

			_audioManager.SfxAudioHandler.PlayOneShot(_FMODEventsConfiguration.GetEventReference(type), pos);
		}

        public void ResolveSingleSoundDemand(Vector3 pos, InteractableSoundType type, bool isOn)
        {
            if (!_isSoundOn)
                return;

            _audioManager.SfxAudioHandler.PlayOneShot(_FMODEventsConfiguration.GetEventReference(type), pos);
        }

        public void ResolveLoopSoundDemand(bool turnOn, InteractableSoundType type)
		{
            if (!_isSoundOn)
                return;

            SoundInstanceHandler instance = default;

			switch (type)
			{
				case InteractableSoundType.WoodenBox_Drag:
					instance = _woodenBoxDrag;
					break;
				default:
					return;
			}

			SwitchPlaybackState(instance, turnOn);
		}

        private void SwitchPlaybackState(SoundInstanceHandler instance, bool turnOn)
        {
            if (turnOn)
			{
				instance.StartLoop();
			}
			else
			{
				instance.StopLoop(STOP_MODE.ALLOWFADEOUT);
			}
        }

        public void StopAll()
		{
			_woodenBoxDrag.StopLoop(STOP_MODE.IMMEDIATE);
		}

    }
}