using ObjectPool;
using UnityEngine;

namespace VFX
{
    public abstract class VFXObjectBase : PoolObjectBase
    {
        [SerializeField] private ParticleSystem _vfx;
        [SerializeField] private ParticleSystemCallback _callback;


        #region Unity Methods
        private void Awake()
        {
            _callback.OnParticleSystemFinished += ParticleFished;
        }

        private void OnEnable()
        {
            _vfx.Stop();
            _vfx.Play();
        }

        private void OnDestroy()
        {
            _callback.OnParticleSystemFinished -= ParticleFished;
        }

        #endregion

        public override abstract void ResetObject();

        protected virtual void ParticleFished()
        {
            gameObject.SetActive(false);
        }
    }
}