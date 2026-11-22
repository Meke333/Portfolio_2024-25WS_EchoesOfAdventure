using Interface;
using NaughtyAttributes;
using Player;
using UnityEngine;
using Zenject;

namespace Ability
{
    public class FireAbility : AbilityBase
    {

        [SerializeField, ReadOnly] bool _abilityButtonDown;
        [SerializeField] bool _canHold;

        private float time;
        private Vector2 _originalHitboxPosition;

        private void Awake()
        {
            _originalHitboxPosition = _hitbox.localPosition;
        }

        private void Update()
        {

            if (!isActive)
                return;

            time += Time.deltaTime;

            if (time < characterSettings.FireAbilityStopCooldownSeconds)
                return;

            if (_canHold && _abilityButtonDown)
                return;

            isActive = false;
            _hitbox.localPosition = _originalHitboxPosition;
            //TO-DO: Events Fire Character Stops Spreading Fire

        }
        private void FixedUpdate()
        {
            if (!isActive)
                return;
            EvaluateHitResults(GetHitArray<IBurnable>(Constants.Layers.Interactables));
            EvaluateHitResults(GetHitArray<IBurnable>(Constants.Layers.Echoes));
        }

        public override void UseAbility(Vector2 position)
        {
            _abilityButtonDown = true;

            _hitbox.localPosition = _originalHitboxPosition + position;

            isActive = true;
            time = 0;
        }

        public override void StopAbility()
        {
            _abilityButtonDown = false;

        }

        private void EvaluateHitResults(IBurnable[] burnableList)
        {
            for (int i = 0; i < burnableList.Length; i++)
            {
                if (!burnableList[i].CanBurnDown())
                    continue;


                playerEvents.Ability_Success?.Invoke();
                burnableList[i].BurnDown();
            }
        }
    }
}

