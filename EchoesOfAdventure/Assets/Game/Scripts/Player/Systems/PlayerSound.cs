using FMOD.Studio;
using Sounds;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerSound : PlayerSystem
    {
        [Inject] private SoundManager _audiomanager;
        [Inject] private FMODEventsConfiguration _FMODEventsConfiguration;

        //Toggleable SFXs
        private SoundInstanceHandler _playerSteps;
        private SoundInstanceHandler _tamo_waterwalk;
        private SoundInstanceHandler _cenali_waterwalk;
        private SoundInstanceHandler _tamo_climb;
        private SoundInstanceHandler _cenali_climb;

        private void Start()
        {
            SpawnAllInstances();

            _playerEvents.Sounds_SingleSound += ResolveSingleSoundDemand;
            _playerEvents.Sound_LoopSound += ResolveLoopSoundDemand;
        }

        private void OnDisable()
        {
            StopAll();
        }

        private void OnDestroy()
        {
            StopAll();
                
            _playerEvents.Sounds_SingleSound -= ResolveSingleSoundDemand;
            _playerEvents.Sound_LoopSound -= ResolveLoopSoundDemand;
        }

        public void SpawnAllInstances()
        {
            _playerSteps = _audiomanager.FactorySoundAudioHandler.CreateEventInstance(_FMODEventsConfiguration.GetEventReference(PlayerSoundType.Player_Steps));
            _tamo_waterwalk = _audiomanager.FactorySoundAudioHandler.CreateEventInstance(_FMODEventsConfiguration.GetEventReference(PlayerSoundType.Tamo_WaterWalk));
            _cenali_waterwalk = _audiomanager.FactorySoundAudioHandler.CreateEventInstance(_FMODEventsConfiguration.GetEventReference(PlayerSoundType.Cenali_WaterWalk));
            _tamo_climb = _audiomanager.FactorySoundAudioHandler.CreateEventInstance(_FMODEventsConfiguration.GetEventReference(PlayerSoundType.Tamo_Climb));
            _cenali_climb = _audiomanager.FactorySoundAudioHandler.CreateEventInstance(_FMODEventsConfiguration.GetEventReference(PlayerSoundType.Cenali_Climb));
        }

        public void StopAll()
        {
            _playerSteps.StopLoop(STOP_MODE.IMMEDIATE);
            _tamo_waterwalk.StopLoop(STOP_MODE.IMMEDIATE);
            _cenali_waterwalk.StopLoop(STOP_MODE.IMMEDIATE);
            _tamo_climb.StopLoop(STOP_MODE.IMMEDIATE);
            _cenali_climb.StopLoop(STOP_MODE.IMMEDIATE);
        }

        public void ResolvePlayerSoundType(PlayerSoundType type, bool triggerOn = false)
        {
            switch (type)
            {
                case PlayerSoundType.Character_Jump:
                case PlayerSoundType.Character_Land:
                case PlayerSoundType.Mukao_FireJump:
                case PlayerSoundType.Tamo_Ability:
                case PlayerSoundType.Tamo_TurnToStone:
                case PlayerSoundType.Cenali_Ability:
                case PlayerSoundType.Canali_GrowingVine:
                case PlayerSoundType.Mukao_Ability:
                    ResolveSingleSoundDemand(type);
                    break;
                case PlayerSoundType.Player_Steps:
                case PlayerSoundType.Tamo_Climb:
                case PlayerSoundType.Tamo_WaterWalk:
                case PlayerSoundType.Cenali_Climb:
                case PlayerSoundType.Cenali_WaterWalk:
                    ResolveLoopSoundDemand(type, triggerOn);
                    break;
            }
        }

        public void ResolveSingleSoundDemand(PlayerSoundType type)
        {
            if (!gameObject.activeSelf)
                return;

            //To-Do: Play Sound One Time
            _audiomanager.SfxAudioHandler.PlayOneShot(_FMODEventsConfiguration.GetEventReference(type), transform.position);
        }

        public void ResolveLoopSoundDemand(PlayerSoundType type, bool isOn)
        {

            if (!gameObject.activeSelf)
            {
                return;
            }

            SoundInstanceHandler instance = default;

            //To-Do: Turn On/Off Sound
            switch (type)
            {
                case PlayerSoundType.Tamo_Climb:
                    instance = _tamo_climb;
                    break;
                case PlayerSoundType.Tamo_WaterWalk:
                    instance = _tamo_waterwalk;
                    break;
                case PlayerSoundType.Cenali_Climb:
                    instance = _cenali_climb;
                    break;
                case PlayerSoundType.Cenali_WaterWalk:
                    instance = _cenali_waterwalk;
                    break;
                case PlayerSoundType.Player_Steps:
                    instance = _playerSteps;
                    break;
                default:
                    return;
            }

            if (instance == null)
                return;


            SwitchPlaybackState(instance, isOn);

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
    }
}

