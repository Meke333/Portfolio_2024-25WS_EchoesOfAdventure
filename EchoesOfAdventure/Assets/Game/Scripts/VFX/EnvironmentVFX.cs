
using Events;
using Interface;
using System;
using UnityEngine;
using Zenject;

namespace VFX
{
    public class EnvironmentVFX : MonoBehaviour
	{
		[Inject] private ProjectEvents _projectEvents;

		[Header("Landing VFX")]
        [SerializeField] private LandingVFXManager _landingVFXManager;

        #region Unity Methods
        private void OnEnable()
        {
            _projectEvents.VFX_Landing += PlayVFXLanding;
        }

        private void OnDisable()
        {
            _projectEvents.VFX_Landing += PlayVFXLanding;
        }
        #endregion

        private void PlayVFXLanding(Vector3 vector, float velocity)
        {
            if (_landingVFXManager == null)
                return;

            _landingVFXManager.Activate(vector, velocity);
        }
    }
}