using ObjectPool;
using UnityEngine;
using Zenject;

namespace VFX
{
    public class LandingVFXManager : VFXManagerBase<LandingVFX>
    {


        protected override void Awake()
        {
            base.Awake();
            SetValues(transform, generalPrefabConfigurations.LandingVFX, _objectAmount);
        }

        public void Activate(Vector3 vector, float velocity)
        {

            LandingVFX activated = (LandingVFX) ActivateAndPlace(vector);
            activated.ResetObject();
            activated.SetScale(generalPrefabConfigurations.LandingVFXAirtimeCurve.Evaluate(velocity));
        }
    }
}