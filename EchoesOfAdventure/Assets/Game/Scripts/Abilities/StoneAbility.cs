using Interface;
using System;
using UnityEngine;

namespace Ability
{
    public class StoneAbility : AbilityBase
    {

#if DEBUG || UNITY_EDITOR
        [SerializeField] private MeshRenderer mesh;
#endif

        private void FixedUpdate()
        {

#if DEBUG || UNITY_EDITOR
            mesh.enabled = isActive;
#endif

        }

        public override void UseAbility(Vector2 position)
        {
            _hitbox.localPosition = position;
            EvaluateHitResults(GetHitArray<IStoneable>(Constants.Layers.Interactables));

        }


        public override void StopAbility()
        {
            return;
        }

        private void EvaluateHitResults(IStoneable[] stoneList)
        {
            for(int i = 0; i < stoneList.Length; i++)
            {
                playerEvents.Ability_Success?.Invoke();
                stoneList[i].TurnToStone();
            }
        }
    }
}

