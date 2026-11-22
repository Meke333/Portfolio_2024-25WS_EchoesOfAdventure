
using System;
using UnityEngine;

namespace VFX
{
	public class ParticleSystemCallback : MonoBehaviour
	{
		public Action OnParticleSystemFinished;

        private void OnParticleSystemStopped()
        {
            OnParticleSystemFinished?.Invoke();
        }
    }
}