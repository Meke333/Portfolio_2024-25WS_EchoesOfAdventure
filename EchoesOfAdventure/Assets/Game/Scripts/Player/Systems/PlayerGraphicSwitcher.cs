using UnityEngine;

namespace Player
{
    public class PlayerGraphicSwitcher : PlayerSystem
    {
        [SerializeField] private GameObject _normalGraphicGameObject;
        [SerializeField] private GameObject _climbGraphicGameObject;


        #region Unity Method

        private void OnEnable()
        {
            if (_climbGraphicGameObject == null)
                return;

            _playerEvents.State_DistributeUpdatedPlayerState += UpdatePlayerState;
        }

        private void OnDisable()
        {
            if (_climbGraphicGameObject == null)
                return;

            _playerEvents.State_DistributeUpdatedPlayerState -= UpdatePlayerState;
        }

        #endregion

        private void UpdatePlayerState(PlayerStateType type)
        {

            bool _useClimbingGameObject = false;

            switch (type)
            {
                case PlayerStateType.Idle:
                case PlayerStateType.Dead:
                case PlayerStateType.Jumping:
                case PlayerStateType.Falling:
                case PlayerStateType.Grabbing:
                    break;
                case PlayerStateType.Climbing:
                    _useClimbingGameObject = true;
                    break;
            }

            _normalGraphicGameObject.SetActive(!_useClimbingGameObject);
            _climbGraphicGameObject.SetActive(_useClimbingGameObject);

        }



    }
}