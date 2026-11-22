
using Echo;
using Events;
using FMODUnity;
using Player;
using Sounds;
using System;
using UnityEngine;
using Zenject;

namespace Interactable
{
    [RequireComponent(typeof(StudioEventEmitter))]
	public class InteractableSoundEmitter : MonoBehaviour
	{
        [Inject] private ProjectEvents _projectEvents;

		[Inject] private SoundManager _audioManager;
		[Inject] private FMODEventsConfiguration _soundConfiguration;

		[SerializeField] private InteractableSoundType _soundType;
        [SerializeField] private EchoSoundType _echoSoundType;
        [SerializeField] private CharacterType _normalEchoType;
        [SerializeField] private GameObject _emitterObject;

		private StudioEventEmitter _emitter;

        private void Start()
        {
			_emitter = _audioManager.FactorySoundAudioHandler
				.InitializeEventEmitter(_soundConfiguration.GetEventReference(_soundType),
				_emitterObject);


            _projectEvents.Character_CharacterSelected += UpdateCharacterSelected;
            
            if (_emitter != null)
                _emitter.Play();
        }

        private void OnDestroy()
        {
            _projectEvents.Character_CharacterSelected -= UpdateCharacterSelected;
            if (_emitter != null)
                _emitter.Stop();
        }

        private void UpdateCharacterSelected(CharacterType type)
        {
            if (_echoSoundType == default)
                return;

            _emitter.Stop();
            if (_normalEchoType == type)
            {
                _emitter = _audioManager.FactorySoundAudioHandler
                .InitializeEventEmitter(_soundConfiguration.GetEventReference(_soundType),
                _emitterObject);
            }
            else
            {
                _emitter = _audioManager.FactorySoundAudioHandler
                .InitializeEventEmitter(_soundConfiguration.GetEventReference(_echoSoundType),
                _emitterObject);
            }
            _emitter.Play();
        }
    }
}