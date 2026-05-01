using System;

namespace UI.Runtime
{
    public sealed class UIShortcutCloseAllBridge : IDisposable
    {
        private readonly MainInputUIGameplayGate _inputGate;
        private readonly IUIService _uiService;

        public UIShortcutCloseAllBridge(MainInputUIGameplayGate inputGate, IUIService uiService)
        {
            _inputGate = inputGate;
            _uiService = uiService;
            _inputGate.CloseAllShortcutRequested += OnCloseAllShortcutRequested;
        }

        public void Dispose()
        {
            _inputGate.CloseAllShortcutRequested -= OnCloseAllShortcutRequested;
        }

        private void OnCloseAllShortcutRequested()
        {
            _uiService.HandleCloseAllShortcut();
        }
    }
}
