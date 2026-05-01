using System.Collections.Generic;

namespace UI.Runtime
{
    public sealed class UIPanelRegistry : IUIPanelRegistry
    {
        private readonly Dictionary<PanelId, UIPanelRegistration> _registrationById = new();

        public void Register(UIPanelRegistration registration)
        {
            _registrationById[registration.PanelId] = registration;
        }

        public bool TryGetById(PanelId panelId, out UIPanelRegistration registration)
        {
            return _registrationById.TryGetValue(panelId, out registration);
        }

        public bool TryGetByType<TPanel>(out UIPanelRegistration registration) where TPanel : class, IUIPanel
        {
            return _registrationById.TryGetValue(PanelId.From<TPanel>(), out registration);
        }
    }
}
