using System;

namespace UI.Runtime
{
    public struct UIPanelRegistration
    {
        public PanelId PanelId { get; private set; }
        public Type PanelType { get; private set; }
        public string AssetPath { get; private set; }

        public UIPanelRegistration(PanelId panelId, Type panelType, string assetPath)
        {
            PanelId = panelId;
            PanelType = panelType;
            AssetPath = assetPath;
        }

        public static UIPanelRegistration Create<TPanel>(string assetPath) where TPanel : class, IUIPanel
        {
            return new UIPanelRegistration(PanelId.From<TPanel>(), typeof(TPanel), assetPath);
        }
    }
}
