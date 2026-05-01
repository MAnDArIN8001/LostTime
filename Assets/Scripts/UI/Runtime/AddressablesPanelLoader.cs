using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UI.Runtime
{
    public sealed class AddressablesPanelLoader : IResourceLoader
    {
        public GameObject LoadPanelPrefab(PanelId panelId, string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                Debug.LogError($"[AddressablesPanelLoader] Empty assetPath for panel {panelId}.");
                return null;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(assetPath);
            var prefab = handle.WaitForCompletion();

            if (prefab == null || handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[AddressablesPanelLoader] Failed to load '{assetPath}' for panel {panelId}.");
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }

                return null;
            }

            return prefab;
        }

        public void ReleasePanelPrefab(PanelId panelId, GameObject prefab)
        {
            if (prefab == null)
            {
                return;
            }

            Addressables.Release(prefab);
        }
    }
}
