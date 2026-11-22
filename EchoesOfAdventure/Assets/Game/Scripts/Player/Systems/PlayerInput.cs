using Events;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;


namespace Player
{
    public class PlayerInput : PlayerSystem
    {
        [Inject] private ProjectEvents _projectEvents;

        private InputActionMap _gameplayActionMap;

        private bool shouldInputTrigger;

#if DEBUG || UNITY_EDITOR
        [SerializeField, ReadOnly, Foldout("Debug: Input-Values")] private Vector2 moveInput;
        [SerializeField, ReadOnly, Foldout("Debug: Input-Values")] private bool interactInput;
        [SerializeField, ReadOnly, Foldout("Debug: Input-Values")] private bool jumpLastInput;
        [SerializeField, ReadOnly, Foldout("Debug: Input-Values")] private bool useAilityInput;
        

#endif

        #region UnityMethods
        public void Awake()
        {
            _gameplayActionMap = InputSystem.actions.FindActionMap("Gameplay");

            _projectEvents.Input_TogglePlayerInput += TogglePlayerInput;
        }

        private void OnEnable()
        {
            InputSystem.actions["Move"].performed += OnMoveInput;
            InputSystem.actions["Move"].canceled += OnMoveInput;
            InputSystem.actions["Interact"].started += OnInteractInputStarted;
            InputSystem.actions["Interact"].performed += OnInteractInputPerformed;
            InputSystem.actions["Jump"].started += OnJumpInputStarted;
            InputSystem.actions["Jump"].canceled += OnJumpInputPerformed;
            InputSystem.actions["UseAbility"].started += OnUseAbilityInputStarted;
            InputSystem.actions["UseAbility"].canceled += OnUseAbilityInputPerformed;
        }

        private void OnDisable()
        {
            InputSystem.actions["Move"].performed -= OnMoveInput;
            InputSystem.actions["Move"].canceled -= OnMoveInput;
            InputSystem.actions["Interact"].performed -= OnInteractInputStarted;
            InputSystem.actions["Interact"].canceled -= OnInteractInputPerformed;
            InputSystem.actions["Jump"].started -= OnJumpInputStarted;
            InputSystem.actions["Jump"].canceled -= OnJumpInputPerformed;
            InputSystem.actions["UseAbility"].started -= OnUseAbilityInputStarted;
            InputSystem.actions["UseAbility"].canceled -= OnUseAbilityInputPerformed;
        }

        private void OnDestroy()
        {
            _projectEvents.Input_TogglePlayerInput -= TogglePlayerInput;
        }
        #endregion

        private void TogglePlayerInput(bool isOn)
        {
            shouldInputTrigger = isOn;
        }

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            if (!shouldInputTrigger)
                return;

            _playerEvents.Input_OnMoveInput?.Invoke(context.ReadValue<Vector2>());
#if DEBUG || UNITY_EDITOR
            moveInput = context.ReadValue<Vector2>();
#endif
        }

        private void OnInteractInputStarted(InputAction.CallbackContext context)
        {
            if (!shouldInputTrigger)
                return;

            _playerEvents.Input_OnInteractInput?.Invoke(true);
#if DEBUG || UNITY_EDITOR
            interactInput = true;
#endif
        }
        private void OnInteractInputPerformed(InputAction.CallbackContext context)
        {
            if (!shouldInputTrigger)
                return;

            _playerEvents.Input_OnInteractInput?.Invoke(false);
#if DEBUG || UNITY_EDITOR
            interactInput = false;
#endif
        }
        private void OnJumpInputStarted(InputAction.CallbackContext context)
        {
            if (!shouldInputTrigger)
                return;

            _playerEvents.Input_OnJumpInput?.Invoke(true);
#if DEBUG || UNITY_EDITOR
            jumpLastInput = true;
#endif
        }
        private void OnJumpInputPerformed(InputAction.CallbackContext context)
        {
            if (!shouldInputTrigger)
                return;

            _playerEvents.Input_OnJumpInput?.Invoke(false);
#if DEBUG || UNITY_EDITOR
            jumpLastInput = false;
#endif
        }

        private void OnUseAbilityInputStarted(InputAction.CallbackContext context)
        {
            if (!shouldInputTrigger)
                return;

            _playerEvents.Input_OnUseAbilityInput?.Invoke(true);
#if DEBUG || UNITY_EDITOR
            useAilityInput = true;
#endif
        }
        private void OnUseAbilityInputPerformed(InputAction.CallbackContext context)
        {
            if (!shouldInputTrigger)
                return;

            _playerEvents.Input_OnUseAbilityInput?.Invoke(false);
#if DEBUG || UNITY_EDITOR
            useAilityInput = false;
#endif
        }
    }
}

