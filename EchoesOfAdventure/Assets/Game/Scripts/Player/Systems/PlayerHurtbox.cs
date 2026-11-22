using Interface;
using System;
using Events;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerHurtbox : PlayerSystem, IHurtBox
    {
        [Inject] private ProjectEvents _projectEvents;
        
        private bool _isActive = true;

        public bool CanGetHurt<T>(T layers) where T : Enum
        {
            if (typeof(T) != typeof(CharacterMask))
                return false;

            if (!_isActive)
                return false;

            CharacterMask a = (CharacterMask) Enum.Parse(typeof(CharacterMask), layers.ToString());

            if ((a & _player.CharacterMask) == _player.CharacterMask)
                return true;

            return false;
        }

        public void GetDamage()
        {
            _projectEvents.LoadToCurrentRun?.Invoke();
            Debug.Log("Aua");
        }
    }
}