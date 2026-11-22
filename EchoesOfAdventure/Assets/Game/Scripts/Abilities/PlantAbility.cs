using Interface;
using System;
using UnityEngine;

namespace Ability
{
    public class PlantAbility : AbilityBase
    {
#if DEBUG || UNITY_EDITOR
        [SerializeField] private MeshRenderer mesh;
#endif

        private float time;

        private void FixedUpdate()
        {
#if DEBUG || UNITY_EDITOR
            mesh.enabled = isActive;
#endif
        }


        public override void UseAbility(Vector2 position)
        {
            EvaluateHitResults(GetHitArray<ISproutable>(Constants.Layers.Interactables));
        }


        public override void StopAbility()
        {
            return;
        }

        private void EvaluateHitResults(ISproutable[] sproutableList)
        {
            for (int i = 0; i < sproutableList.Length; i++)
            {

                playerEvents.Ability_Success?.Invoke();
                sproutableList[i].Sprout();
            }
        }
    }
}

