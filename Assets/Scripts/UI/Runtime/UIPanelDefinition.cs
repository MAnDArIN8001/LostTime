using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.Runtime
{
    [CreateAssetMenu(
        fileName = "UIPanelDefinition",
        menuName = "LostTime/UI/UI Panel Definition")]
    public sealed class UIPanelDefinition : ScriptableObject
    {
        [SerializeField] private string _panelTypeName;
        [SerializeField] private GameObject _panelPrefab;
        [SerializeField] private string _assetPathOrKey;

#if UNITY_EDITOR
        [SerializeField] private MonoScript _panelScript;
#endif

        public string PanelTypeName => _panelTypeName;
        public GameObject PanelPrefab => _panelPrefab;
        public string AssetPathOrKey => _assetPathOrKey;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_panelScript == null)
            {
                return;
            }

            var type = _panelScript.GetClass();
            _panelTypeName = type == null ? string.Empty : type.FullName;
        }
#endif
    }
}
