using System;

namespace UI.Runtime
{
    public interface IUIService : IDisposable
    {
        public TPanel Open<TPanel>() where TPanel : class, IUIPanel;
        public bool Close<TPanel>(UIPanelCloseReason reason = UIPanelCloseReason.User) where TPanel : class, IUIPanel;
        public bool Close(PanelId panelId, UIPanelCloseReason reason = UIPanelCloseReason.User);
        public bool CloseTop(UIPanelCloseReason reason = UIPanelCloseReason.User);
        public void CloseAll(UIPanelCloseReason reason = UIPanelCloseReason.CloseAll);
        public bool HandleCloseAllShortcut();
        public bool IsOpen<TPanel>() where TPanel : class, IUIPanel;
        public bool TryGet<TPanel>(out TPanel panel) where TPanel : class, IUIPanel;
    }
}
