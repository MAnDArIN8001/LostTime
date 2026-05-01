using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Runtime
{
    [CreateAssetMenu(
        fileName = "UIRuntimeConfig",
        menuName = "LostTime/UI/UI Runtime Config")]
    public sealed class UIRuntimeConfig : ScriptableObject
    {
        [field: SerializeField] public UIResourceBackend ResourceBackend { get; private set; } = UIResourceBackend.Resources;
        
        [Space, SerializeField] private List<UIPanelConfigEntry> _panelEntries = new();

        public IReadOnlyList<UIPanelConfigEntry> PanelEntries => _panelEntries;
    }

    public enum UIResourceBackend
    {
        Resources = 0,
        Addressables = 1
    }

    [Serializable]
    public struct UIPanelConfigEntry
    {
        [SerializeField] private string _panelTypeName;
        [SerializeField] private string _assetPath;

        public string PanelTypeName => _panelTypeName;
        public string AssetPath => _assetPath;
    }
}
