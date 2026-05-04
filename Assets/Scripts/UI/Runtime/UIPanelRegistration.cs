using System;
using UnityEngine;

namespace UI.Runtime
{
    public struct UIPanelRegistration
    {
        public PanelId PanelId { get; private set; }
        public Type PanelType { get; private set; }
        public string AssetPath { get; private set; }
        public GameObject Prefab { get; private set; }

        public UIPanelRegistration(PanelId panelId, Type panelType, string assetPath, GameObject prefab = null)
        {
            PanelId = panelId;
            PanelType = panelType;
            AssetPath = assetPath;
            Prefab = prefab;
        }

        public static UIPanelRegistration Create<TPanel>(string assetPath, GameObject prefab = null) where TPanel : class, IUIPanel
        {
            return new UIPanelRegistration(PanelId.From<TPanel>(), typeof(TPanel), assetPath, prefab);
        }
    }
}
