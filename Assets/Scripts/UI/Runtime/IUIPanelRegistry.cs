namespace UI.Runtime
{
    public interface IUIPanelRegistry
    {
        public void Register(UIPanelRegistration registration);
        public bool TryGetById(PanelId panelId, out UIPanelRegistration registration);
        public bool TryGetByType<TPanel>(out UIPanelRegistration registration) where TPanel : class, IUIPanel;
    }
}
