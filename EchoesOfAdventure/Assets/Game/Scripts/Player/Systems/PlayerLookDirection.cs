using NaughtyAttributes;
using System;
using UnityEngine;

namespace Player
{
    public class PlayerLookDirection : PlayerSystem
    {
        [SerializeField, ReadOnly] private int _lookDirection = 1;

        private void OnEnable()
        {
            _playerEvents.Input_OnMoveInput += MoveInput;
        }

        private void OnDisable()
        {
            _playerEvents.Input_OnMoveInput -= MoveInput;
        }

        private void MoveInput(Vector2 vector)
        {
            if (vector.magnitude <= 0)
                return;

            int temp = Mathf.RoundToInt(vector.x);

            if (temp == 0)
                return;

            int directionInput = temp;

            _lookDirection = directionInput;
            _playerEvents.LookDirection_OnLookingDirectionChange?.Invoke(_lookDirection);
        }
    }

}
