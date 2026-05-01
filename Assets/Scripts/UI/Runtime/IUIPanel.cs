namespace UI.Runtime
{
    public interface IUIPanel
    {
        PanelId Id { get; }
        UIPanelConfig Config { get; }
        bool IsVisible { get; }

        void Show();
        void Hide();
        void Close(UIPanelCloseReason reason);
    }
}
