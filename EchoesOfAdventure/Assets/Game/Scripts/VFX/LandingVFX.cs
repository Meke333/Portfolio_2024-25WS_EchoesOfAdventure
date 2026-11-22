using Events;
using System;
using UnityEngine;
using Zenject;

namespace VFX
{
    public class LandingVFX : VFXObjectBase
    { 
        [SerializeField] private Transform _vfxTransform;

        public override void ResetObject()
        {
            _vfxTransform.localScale = Vector3.one;    
        }

        public void SetScale(float size)
        {
            _vfxTransform.localScale = Vector3.one * size;
        }
    }
}