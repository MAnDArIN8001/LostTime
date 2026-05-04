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

        [Space, SerializeField] private List<UIPanelDefinition> _panelDefinitions = new();

        public IReadOnlyList<UIPanelDefinition> PanelDefinitions => _panelDefinitions;
    }

    public enum UIResourceBackend
    {
        Resources = 0,
        Addressables = 1
    }

}
