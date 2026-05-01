using System.Collections.Generic;

namespace UI.Runtime
{
    public sealed class UIService : IUIService
    {
        private readonly IUIPanelFactory _panelFactory;
        private readonly IUIInputGate _uiInputGate;
        private readonly Dictionary<PanelId, IUIPanel> _openPanelById = new();
        private readonly Dictionary<PanelId, IUIPanel> _keepAliveCacheById = new();
        private readonly List<PanelId> _panelStack = new();
        private bool _isDisposed;
        private bool _isGameplayInputBlocked;

        public UIService(IUIPanelFactory panelFactory, IUIInputGate uiInputGate = null)
        {
            _panelFactory = panelFactory;
            _uiInputGate = uiInputGate;
        }

        public TPanel Open<TPanel>() where TPanel : class, IUIPanel
        {
            if (_isDisposed)
            {
                return null;
            }

            var panelId = PanelId.From<TPanel>();
            if (_openPanelById.TryGetValue(panelId, out var alreadyOpenPanel))
            {
                EnsurePanelAtTop(panelId);
                if (!alreadyOpenPanel.IsVisible)
                {
                    alreadyOpenPanel.Show();
                }

                return alreadyOpenPanel as TPanel;
            }

            IUIPanel panel = null;

            if (_keepAliveCacheById.TryGetValue(panelId, out var cachedPanel))
            {
                panel = cachedPanel;
                _keepAliveCacheById.Remove(panelId);
            }
            else
            {
                panel = _panelFactory.Create<TPanel>();
            }

            if (panel == null)
            {
                return null;
            }

            panel.Show();
            _openPanelById[panelId] = panel;
            EnsurePanelAtTop(panelId);
            SyncGameplayInputGate();
            return panel as TPanel;
        }

        public bool Close<TPanel>(UIPanelCloseReason reason = UIPanelCloseReason.User) where TPanel : class, IUIPanel
        {
            return Close(PanelId.From<TPanel>(), reason);
        }

        public bool Close(PanelId panelId, UIPanelCloseReason reason = UIPanelCloseReason.User)
        {
            if (_isDisposed)
            {
                return false;
            }

            if (!_openPanelById.TryGetValue(panelId, out var panel))
            {
                return false;
            }

            panel.Close(reason);
            _openPanelById.Remove(panelId);
            RemoveFromStack(panelId);

            if (panel.Config.CachePolicy == UICachePolicy.KeepAlive)
            {
                _keepAliveCacheById[panelId] = panel;
                SyncGameplayInputGate();
                return true;
            }

            _panelFactory.Release(panelId);
            SyncGameplayInputGate();
            return true;
        }

        public bool CloseTop(UIPanelCloseReason reason = UIPanelCloseReason.User)
        {
            if (_isDisposed || _panelStack.Count == 0)
            {
                return false;
            }

            var topPanelId = _panelStack[_panelStack.Count - 1];
            return Close(topPanelId, reason);
        }

        public void CloseAll(UIPanelCloseReason reason = UIPanelCloseReason.CloseAll)
        {
            if (_isDisposed)
            {
                return;
            }

            while (_panelStack.Count > 0)
            {
                var topPanelId = _panelStack[_panelStack.Count - 1];
                Close(topPanelId, reason);
            }

            SyncGameplayInputGate();
        }

        public bool HandleCloseAllShortcut()
        {
            if (_isDisposed || _panelStack.Count == 0)
            {
                return false;
            }

            CloseAll(UIPanelCloseReason.CloseAll);
            return true;
        }

        public bool IsOpen<TPanel>() where TPanel : class, IUIPanel
        {
            if (_isDisposed)
            {
                return false;
            }

            return _openPanelById.ContainsKey(PanelId.From<TPanel>());
        }

        public bool TryGet<TPanel>(out TPanel panel) where TPanel : class, IUIPanel
        {
            panel = null;
            if (_isDisposed)
            {
                return false;
            }

            if (!_openPanelById.TryGetValue(PanelId.From<TPanel>(), out var openPanel))
            {
                return false;
            }

            panel = openPanel as TPanel;
            return panel != null;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            CloseAll(UIPanelCloseReason.Dispose);

            var cachedIds = new List<PanelId>(_keepAliveCacheById.Keys);
            for (var i = 0; i < cachedIds.Count; i++)
            {
                _panelFactory.Release(cachedIds[i]);
            }

            _keepAliveCacheById.Clear();
            _openPanelById.Clear();
            _panelStack.Clear();
            RestoreGameplayInputIfNeeded();
            _isDisposed = true;
        }

        private void EnsurePanelAtTop(PanelId panelId)
        {
            RemoveFromStack(panelId);
            _panelStack.Add(panelId);
        }

        private void RemoveFromStack(PanelId panelId)
        {
            for (var i = _panelStack.Count - 1; i >= 0; i--)
            {
                if (_panelStack[i] == panelId)
                {
                    _panelStack.RemoveAt(i);
                    return;
                }
            }
        }

        private void SyncGameplayInputGate()
        {
            if (_uiInputGate == null)
            {
                return;
            }

            if (ShouldBlockGameplayInput())
            {
                if (_isGameplayInputBlocked)
                {
                    return;
                }

                _uiInputGate.BlockGameplayInput();
                _isGameplayInputBlocked = true;
                return;
            }

            RestoreGameplayInputIfNeeded();
        }

        private bool ShouldBlockGameplayInput()
        {
            foreach (var pair in _openPanelById)
            {
                if (pair.Value != null && pair.Value.Config.BlocksGameplayInput)
                {
                    return true;
                }
            }

            return false;
        }

        private void RestoreGameplayInputIfNeeded()
        {
            if (_uiInputGate == null || !_isGameplayInputBlocked)
            {
                return;
            }

            _uiInputGate.RestoreGameplayInput();
            _isGameplayInputBlocked = false;
        }
    }
}
