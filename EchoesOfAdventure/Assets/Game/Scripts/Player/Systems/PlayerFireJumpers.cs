
using Interface;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerFireJumpers : PlayerSystem, IFireJumpable
    {
        [Inject] private InteractableConfigurations _interactableConfigurations;

        private float _jumpTime;
        private float jumpTime
        {
            get => _jumpTime;
            set
            {
                if (value < 0)
                {
                    return;
                }
                else if (value >= _interactableConfigurations.TimeBeforeHigherJumpFades)
                {
                    _playerEvents.Movement_ActivateHigherJump?.Invoke(false);
                    return;
                }

                _jumpTime = value;
            }
        }

        private bool _isInFire; 

        private void Update()
        {
            jumpTime = !_isInFire ? jumpTime + Time.deltaTime : 0;
        }

        public void SetFireJumpValue(bool isInFire)
        {
            if (isInFire)
            {
                _playerEvents.Movement_ActivateHigherJump?.Invoke(true);
            }

            _isInFire = isInFire;
        }
    }
}