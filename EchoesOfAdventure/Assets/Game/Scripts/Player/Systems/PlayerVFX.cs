using Helper;
using Player;
using UnityEngine;

public class PlayerVFX : PlayerSystem
{
    [SerializeField] private Transform _vfxParent;
    [SerializeField] private GameObject _abilityVFX;

    [SerializeField] private bool _abilityVFXLoops;
    [SerializeField] private bool _shouldUseMoveInput;

    //y axis
    private Vector3 _left = new Vector3(0, 180, 0);
    private Vector3 _right = new Vector3(0, 0, 0);

    //z axis
    private Vector3 _up = new Vector3(0, 0, 45);
    private Vector3 _down = new Vector3(0, 0, -45);
    private Vector2 _savedInput;


    private void OnEnable()
    {
        _playerEvents.Ability_UseAbility += UseAbility;

        if (_shouldUseMoveInput)
            _playerEvents.Input_OnMoveInput += MoveInput;


    }


    private void OnDisable()
    {
        _playerEvents.Ability_UseAbility -= UseAbility;

        if (_shouldUseMoveInput)
            _playerEvents.Input_OnMoveInput -= MoveInput;
    }

    private void UseAbility(bool isOn)
    {
        if (!_abilityVFXLoops && !isOn)
            return;

        if (!_abilityVFXLoops && _abilityVFX.activeSelf == true)
            return;

        _abilityVFX.SetActive(isOn);

        if (_shouldUseMoveInput)
        {
            SetDirectionAccordingToMove(_savedInput);
        }
        else
        {
            SetDirectionOnLook(_player.lookDirection);
        }
    }

    private void MoveInput(Vector2 vector)
    {
        _savedInput = vector;
    }

    private void SetDirectionOnLook(int direction)
    {
        switch (direction)
        {
            case -1:
                _vfxParent.rotation = Quaternion.Euler(_left);
                break;
            case 1:
                _vfxParent.rotation = Quaternion.Euler(_right);
                break;
        }
    }

    private void SetDirectionAccordingToMove(Vector2 vector)
    {
        Vector2 manipulated = DetectDirectionHelper.Get4DirectionVector(vector);

        if (manipulated.magnitude <= 0)
        {
            _vfxParent.rotation = Quaternion.Euler(_player.lookDirection > 0 ? _right : _left);

            return;
        }



        if (Mathf.Abs(manipulated.x) > Mathf.Abs(manipulated.y))
        {
            _vfxParent.rotation = Quaternion.Euler(_player.lookDirection >= 0 ? _right : _left);
        }
        else
        {
            //To-Do: rotate it more to the side and make it dependent on lookdirection

            if (manipulated.y > 0)
            {
                //up
                _vfxParent.rotation = Quaternion.Euler(0, _player.lookDirection > 0 ? _right.y : _left.y, _up.z);
            }
            else
            {
                //down
                _vfxParent.rotation = Quaternion.Euler(0, _player.lookDirection > 0 ? _right.y : _left.y, _down.z);
            }
        }
    }
}
