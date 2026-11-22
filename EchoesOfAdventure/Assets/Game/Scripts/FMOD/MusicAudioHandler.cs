using FMOD.Studio;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Sounds
{
    public class MusicAudioHandler : MonoBehaviour
    {
        [Inject] private SoundManager _audioManager;
        [Inject] private FMODEventsConfiguration configuration;

        private SoundInstanceHandler _music_MainMenu;
        private SoundInstanceHandler _music_Level;

        private SoundInstanceHandler _currentHandler;

        public void InjectConfig(FMODEventsConfiguration config, SoundManager manager)
        {
            configuration = config;
            _audioManager = manager;

            SpawnAllInstances();
        }

        private void Start()
        {
            SpawnAllInstances();

            PlayAudio(MusicType.Music_Level);
        }

        public void SpawnAllInstances()
        {
            _music_MainMenu = _audioManager.FactorySoundAudioHandler.CreateEventInstance(configuration.GetEventReference(MusicType.Music_MainMenu));
            _music_Level = _audioManager.FactorySoundAudioHandler.CreateEventInstance(configuration.GetEventReference(MusicType.Music_Level));
        }

        public void PlayAudio(MusicType type, bool immediately = false)
        {
            if (_currentHandler != null)
                _currentHandler.Stop(immediately ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);

            _currentHandler = GetSoundInstance(type);
            _currentHandler.Play();
        }

        private SoundInstanceHandler GetSoundInstance(MusicType type)
        {
            switch (type)
            {
                case MusicType.Music_MainMenu:
                    return _music_MainMenu;
                case MusicType.Music_Level:
                    return _music_Level;
            }

            throw new InvalidOperationException("MusicType does not contain it.");
        }

    }
}

