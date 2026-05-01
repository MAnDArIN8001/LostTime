using UnityEngine;

namespace UI.Runtime
{
    public sealed class ResourcesPanelLoader : IResourceLoader
    {
        public GameObject LoadPanelPrefab(PanelId panelId, string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                Debug.LogError($"[ResourcesPanelLoader] Empty assetPath for panel {panelId}.");
                return null;
            }

            var prefab = Resources.Load<GameObject>(assetPath);
            if (prefab == null)
            {
                Debug.LogError($"[ResourcesPanelLoader] Prefab not found at path '{assetPath}' for panel {panelId}.");
            }

            return prefab;
        }

        public void ReleasePanelPrefab(PanelId panelId, GameObject prefab)
        {
            if (prefab == null)
            {
                return;
            }

            Resources.UnloadAsset(prefab);
        }
    }
}
