using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerTest : MonoBehaviour
{
    InputActionMap _gameplayActionMap;

    public void Awake()
    {
        _gameplayActionMap = InputSystem.actions.FindActionMap("Gameplay");

    }

    private void OnEnable()
    {
        InputSystem.actions["Move"].performed += Move;
        InputSystem.actions["Interact"].performed += Interact;
        InputSystem.actions["Jump"].performed += Jump;
        InputSystem.actions["UseAbility"].performed += UseAbility;
    }

    private void UseAbility(InputAction.CallbackContext context)
    {
        Debug.Log("Sprint");
    }
    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
    }

    private void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("Interact");
    }

    private void Move(InputAction.CallbackContext context)
    {
        Debug.Log("Look: " + context.ReadValue<Vector2>());
    }
}
