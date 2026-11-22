using FMOD.Studio;
using System.Threading.Tasks;
using UnityEngine;

namespace Sounds
{
    public class SoundInstanceHandler
    {

        private EventInstance eventInstance;
        private bool _isActive;
        private STOP_MODE _stopmode = STOP_MODE.ALLOWFADEOUT;

        public SoundInstanceHandler(EventInstance instance)
        {
            eventInstance = instance;
        }

        public void Play()
        {
            eventInstance.start();
            _isActive = true;
        }

        public void Stop(STOP_MODE mode = STOP_MODE.IMMEDIATE)
        {
            eventInstance.stop(mode);
            _isActive = false;
        }

        public void StartLoop()
        {
            _isActive = true;
            LoopSoundInstance();
        }

        public void StopLoop(STOP_MODE mode)
        {
            _isActive = false;
            _stopmode = mode;
        }

        public void Release()
        {
            eventInstance.release();
        }

        public async void LoopSoundInstance()
        {
            while (_isActive)
            {
                PLAYBACK_STATE playbackState;
                eventInstance.getPlaybackState(out playbackState);

                if (playbackState == PLAYBACK_STATE.STOPPED)
                    eventInstance.start();

                await Task.Yield();
            }

            eventInstance.stop(_stopmode);
        }
    }
}

