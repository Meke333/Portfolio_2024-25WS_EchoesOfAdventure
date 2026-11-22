using Interactable;
using Sounds;
using UnityEngine;
using Zenject;

namespace Echo
{
    public class EchoSound : MonoBehaviour
    {
        [Inject] private EchoEvents _echoEvents;
        [Inject] private SoundManager _audioManager;
        [Inject] private FMODEventsConfiguration _soundConfiguration;


        #region Unity Methods

        private void OnEnable()
        {
            _echoEvents.Fire_Burning += (x) => { ResolveSingleSoundDemand(x, EchoSoundType.Fire_Ambience); };
        }

        private void OnDisable()
        {
            _echoEvents.Fire_Burning -= (x) => { ResolveSingleSoundDemand(x, EchoSoundType.Fire_Ambience); };
        }

        #endregion

        public void ResolveSingleSoundDemand(Vector3 pos, EchoSoundType type)
        {
            _audioManager.SfxAudioHandler.PlayOneShot(_soundConfiguration.GetEventReference(type), pos);
        }
    }
}