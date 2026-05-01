using UnityEngine;

namespace UI.Runtime
{
    public interface IResourceLoader
    {
        GameObject LoadPanelPrefab(PanelId panelId, string assetPath);
        void ReleasePanelPrefab(PanelId panelId, GameObject prefab);
    }
}
