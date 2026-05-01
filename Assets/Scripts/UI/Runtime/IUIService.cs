namespace UI.Runtime
{
    public interface IUIService
    {
        TPanel Open<TPanel>() where TPanel : class, IUIPanel;
        bool Close<TPanel>(UIPanelCloseReason reason = UIPanelCloseReason.User) where TPanel : class, IUIPanel;
        bool Close(PanelId panelId, UIPanelCloseReason reason = UIPanelCloseReason.User);
        bool CloseTop(UIPanelCloseReason reason = UIPanelCloseReason.User);
        void CloseAll(UIPanelCloseReason reason = UIPanelCloseReason.CloseAll);
        bool IsOpen<TPanel>() where TPanel : class, IUIPanel;
        bool TryGet<TPanel>(out TPanel panel) where TPanel : class, IUIPanel;
    }
}
