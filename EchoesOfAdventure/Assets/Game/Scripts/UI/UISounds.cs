using Events;
using Sounds;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UISounds : MonoBehaviour
    {
        [Inject] private ProjectEvents _projectEvents;
        [Inject] private SoundManager _audioManager;
        [Inject] private FMODEventsConfiguration _soundConfiguration;

        [SerializeField] private Button _button;
        [SerializeField] private Slider _slider;
        [SerializeField] private UISoundType _type;

        #region Unity Methods

        private void Start()
        {
            if (_button != null)
                _button.onClick.AddListener(() => ResolveSingleSoundDemand(_type));

            if (_slider != null)
                _slider.onValueChanged.AddListener((x) => ResolveSingleSoundDemand(_type));


        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(() => ResolveSingleSoundDemand(_type));

            if (_slider != null)
                _slider.onValueChanged.RemoveListener((x) => ResolveSingleSoundDemand(_type));
        }

        #endregion

        public void ResolveSingleSoundDemand(UISoundType type)
        {
            _audioManager.SfxAudioHandler.PlayOneShot(_soundConfiguration.GetEventReference(type), transform.position);
        }


    }
}

