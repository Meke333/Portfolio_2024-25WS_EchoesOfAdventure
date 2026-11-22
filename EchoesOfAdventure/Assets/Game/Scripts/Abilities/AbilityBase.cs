using Interface;
using Player;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Ability
{
    public abstract class AbilityBase : MonoBehaviour, IAbility
    {
        [Inject] protected PlayerConfiguration playerConfiguration;
        [Inject] protected CharacterSettingsConfiguration characterSettings;
        [Inject] protected PlayerEvents playerEvents;

        [SerializeField] protected Transform _hitbox;

        protected bool isActive;

        public abstract void UseAbility(Vector2 position);

        public abstract void StopAbility();

        public virtual void ToggleActivity(bool value)
        {
            isActive = value;
        }

        protected T[] GetHitArray<T>()
        {
            return Collider2DHelper.GetHitArray<T>(_hitbox, Constants.Layers.Interactables);
        }
        protected T[] GetHitArray<T>(int layerNumber)
        {
            return Collider2DHelper.GetHitArray<T>(_hitbox, layerNumber);
        }
    }
}

