using System;
using UnityEngine.InputSystem;

namespace UI.Runtime
{
    public sealed class MainInputUIGameplayGate : IUIInputGate, IDisposable
    {
        private readonly MainInput _mainInput;
        private readonly InputAction _closeAllShortcutAction;
        private bool _isGameplayBlocked;

        public event Action CloseAllShortcutRequested;

        public MainInputUIGameplayGate(MainInput mainInput)
        {
            _mainInput = mainInput;
            _closeAllShortcutAction = new InputAction(
                name: "UiCloseAllShortcut",
                type: InputActionType.Button,
                binding: "<Keyboard>/escape");
            _closeAllShortcutAction.AddBinding("<Gamepad>/start");
            _closeAllShortcutAction.performed += OnCloseAllShortcutPerformed;
        }

        public void BlockGameplayInput()
        {
            if (_isGameplayBlocked)
            {
                return;
            }

            _mainInput.Character.Disable();
            _closeAllShortcutAction.Enable();
            _isGameplayBlocked = true;
        }

        public void RestoreGameplayInput()
        {
            if (!_isGameplayBlocked)
            {
                return;
            }

            _closeAllShortcutAction.Disable();
            _mainInput.Character.Enable();
            _isGameplayBlocked = false;
        }

        public void Dispose()
        {
            RestoreGameplayInput();
            _closeAllShortcutAction.performed -= OnCloseAllShortcutPerformed;
            _closeAllShortcutAction.Dispose();
        }

        private void OnCloseAllShortcutPerformed(InputAction.CallbackContext context)
        {
            CloseAllShortcutRequested?.Invoke();
        }
    }
}
