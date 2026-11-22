using UnityEngine;
using FMODUnity;

namespace Sounds
{
    public class SFXAudioHandler
    {
        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            Debug.Log("aaaaa");
            RuntimeManager.PlayOneShot(sound, worldPos);
        }

    }
}


//CharacterSteps
//Echo Ambience
//Menu Click Sounds
//Music
//Environment Sounds

