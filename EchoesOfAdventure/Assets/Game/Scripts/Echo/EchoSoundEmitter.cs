
using Events;
using FMODUnity;
using Player;
using Sounds;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Echo
{
    [RequireComponent(typeof(StudioEventEmitter))]
	public class EchoSoundEmitter : MonoBehaviour
	{
        [Inject] private ProjectEvents _projectEvents;

        [Inject] private SoundManager _audioManager;
		[Inject] private FMODEventsConfiguration _soundConfiguration;

        [SerializeField] private EchoSoundType _soundType;
        [SerializeField] private EchoSoundType _echoSoundType;
        [SerializeField] private CharacterType _normalEchoType;
        [SerializeField] private GameObject _emitterObject;

		private StudioEventEmitter _emitter;

        private void Start()
        {
            _emitter = _audioManager.FactorySoundAudioHandler
                .InitializeEventEmitter(_soundConfiguration.GetEventReference(_soundType), 
                _emitterObject);
            _emitter.Play();
        }

        private void OnEnable()
        {
            _projectEvents.Character_CharacterSelected += UpdateCharacterSelected;
            if (_emitter != null)
                _emitter.Play();
        }

        private void OnDisable()
        {
            if (_emitter != null)
                _emitter.Stop();
            
            Debug.Log("Stop Echo Emitter");

            _projectEvents.Character_CharacterSelected -= UpdateCharacterSelected;
        }

        private async void UpdateCharacterSelected(CharacterType type)
        {
            if (_emitter !=null)
                _emitter.Stop();

            await Task.Delay(5000);

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

            await Task.Yield();

            if (_emitter != null && gameObject.activeInHierarchy)
                _emitter.Play();
        }
        
    }
}