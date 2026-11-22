using Events;
using FMODUnity;
using Player;
using UnityEngine;
using Zenject;

namespace Sounds
{
    [RequireComponent(typeof(StudioListener))]
    public class CameraListenerHandler : MonoBehaviour
    {
        [Inject] private ProjectEvents _projectEvents;
        [SerializeField] private StudioListener _studioListener;

        private void OnEnable()
        {
            _projectEvents.Character_TransmitCurrent += UpdateCurrentPlayer;
        }

        private void OnDisable()
        {
            _projectEvents.Character_TransmitCurrent -= UpdateCurrentPlayer;
        }

        private void UpdateCurrentPlayer(PlayerBase player)
        {
            _studioListener.attenuationObject = player.gameObject;
        }
    }
}

