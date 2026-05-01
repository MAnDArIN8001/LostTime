namespace UI.Runtime
{
    public interface IUIPanelFactory
    {
        public TPanel Create<TPanel>() where TPanel : class, IUIPanel;
        public IUIPanel Create(PanelId panelId);
        public bool IsCreated(PanelId panelId);
        public bool TryGetCreated(PanelId panelId, out IUIPanel panel);
        public void Release(PanelId panelId);
        public void ReleaseAll();
    }
}
