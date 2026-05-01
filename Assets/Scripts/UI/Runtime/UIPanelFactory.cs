using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UI.Runtime
{
    public sealed class UIPanelFactory : IUIPanelFactory
    {
        private readonly IUIPanelRegistry _registry;
        private readonly IResourceLoader _resourceLoader;
        private readonly DiContainer _container;

        private readonly Transform _panelRoot;
        
        private readonly Dictionary<PanelId, IUIPanel> _createdPanelById = new();
        private readonly Dictionary<PanelId, GameObject> _prefabById = new();

        public UIPanelFactory(
            IUIPanelRegistry registry,
            IResourceLoader resourceLoader,
            DiContainer container,
            Transform panelRoot = null)
        {
            _registry = registry;
            _resourceLoader = resourceLoader;
            _container = container;
            _panelRoot = panelRoot;
        }

        public TPanel Create<TPanel>() where TPanel : class, IUIPanel
        {
            var panelId = PanelId.From<TPanel>();
            var panel = Create(panelId);
            return panel as TPanel;
        }

        public IUIPanel Create(PanelId panelId)
        {
            if (_createdPanelById.TryGetValue(panelId, out var existingPanel))
            {
                return existingPanel;
            }

            if (!_registry.TryGetById(panelId, out var registration))
            {
                Debug.LogError($"[UIPanelFactory] Panel registration not found for id {panelId}.");
                return null;
            }

            var prefab = _resourceLoader.LoadPanelPrefab(panelId, registration.AssetPath);
            if (prefab == null)
            {
                return null;
            }

            var instance = _container.InstantiatePrefab(prefab, _panelRoot);
            if (!instance.TryGetComponent(registration.PanelType, out var panelComponent))
            {
                Debug.LogError(
                    $"[UIPanelFactory] Panel prefab '{registration.AssetPath}' does not contain component {registration.PanelType.Name}.");
                UnityEngine.Object.Destroy(instance);
                _resourceLoader.ReleasePanelPrefab(panelId, prefab);
                return null;
            }

            var panel = panelComponent as IUIPanel;
            if (panel == null)
            {
                Debug.LogError(
                    $"[UIPanelFactory] Component {registration.PanelType.Name} does not implement IUIPanel.");
                UnityEngine.Object.Destroy(instance);
                _resourceLoader.ReleasePanelPrefab(panelId, prefab);
                return null;
            }

            _createdPanelById[panelId] = panel;
            _prefabById[panelId] = prefab;
            return panel;
        }

        public bool IsCreated(PanelId panelId)
        {
            return _createdPanelById.ContainsKey(panelId);
        }

        public bool TryGetCreated(PanelId panelId, out IUIPanel panel)
        {
            return _createdPanelById.TryGetValue(panelId, out panel);
        }

        public void Release(PanelId panelId)
        {
            if (!_createdPanelById.TryGetValue(panelId, out var panel))
            {
                return;
            }

            if (panel is Component panelComponent && panelComponent != null)
            {
                UnityEngine.Object.Destroy(panelComponent.gameObject);
            }

            _createdPanelById.Remove(panelId);

            if (_prefabById.TryGetValue(panelId, out var prefab))
            {
                _resourceLoader.ReleasePanelPrefab(panelId, prefab);
                _prefabById.Remove(panelId);
            }
        }

        public void ReleaseAll()
        {
            var panelIds = new List<PanelId>(_createdPanelById.Keys);
            for (var i = 0; i < panelIds.Count; i++)
            {
                Release(panelIds[i]);
            }
        }
    }
}
