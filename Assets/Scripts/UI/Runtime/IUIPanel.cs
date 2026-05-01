namespace UI.Runtime
{
    public interface IUIPanel
    {
        public PanelId Id { get; }
        public UIPanelConfig Config { get; }
        public bool IsVisible { get; }

        public void Show();
        public void Hide();
        public void Close(UIPanelCloseReason reason);
    }
}
