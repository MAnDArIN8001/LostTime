using System;
using UI.Runtime;
using UnityEngine;
using Zenject;

namespace DI
{
    public class UIInstaller : MonoInstaller
    {
        [Header("Runtime")]
        [SerializeField] private UIRuntimeConfig _runtimeConfig;
        [SerializeField] private Transform _uiRuntimeRoot;

        public override void InstallBindings()
        {
            if (_runtimeConfig == null)
            {
                Debug.LogError("[UIInstaller] Runtime config is not assigned.");
                return;
            }

            BindResourceLoader();

            Container.Bind<IUIPanelRegistry>().To<UIPanelRegistry>().AsSingle();
            Container.Bind<IUIPanelFactory>().To<UIPanelFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<MainInputUIGameplayGate>().AsSingle();
            Container.BindInterfacesTo<UIService>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIShortcutCloseAllBridge>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIPanelRegistryBootstrap>().AsSingle()
                .WithArguments(_runtimeConfig);

            if (_uiRuntimeRoot != null)
            {
                Container.Bind<Transform>().FromInstance(_uiRuntimeRoot).AsSingle();
            }
        }

        private void BindResourceLoader()
        {
            switch (_runtimeConfig.ResourceBackend)
            {
                case UIResourceBackend.Addressables:
                    Container.Bind<IResourceLoader>().To<AddressablesPanelLoader>().AsSingle();
                    break;
                default:
                    Container.Bind<IResourceLoader>().To<ResourcesPanelLoader>().AsSingle();
                    break;
            }
        }

        private sealed class UIPanelRegistryBootstrap : IInitializable
        {
            private readonly IUIPanelRegistry _registry;
            private readonly UIRuntimeConfig _runtimeConfig;

            public UIPanelRegistryBootstrap(IUIPanelRegistry registry, UIRuntimeConfig runtimeConfig)
            {
                _registry = registry;
                _runtimeConfig = runtimeConfig;
            }

            public void Initialize()
            {
                if (_runtimeConfig == null || _runtimeConfig.PanelDefinitions == null)
                {
                    return;
                }

                var definitions = _runtimeConfig.PanelDefinitions;
                for (var i = 0; i < definitions.Count; i++)
                {
                    var definition = definitions[i];
                    if (definition == null)
                    {
                        Debug.LogError($"[UIInstaller] Panel definition at index {i} is null.");
                        continue;
                    }

                    var panelType = ResolvePanelType(definition.PanelTypeName);

                    if (panelType == null)
                    {
                        Debug.LogError($"[UIInstaller] Unknown panel type '{definition.PanelTypeName}'.");
                        continue;
                    }

                    if (!typeof(IUIPanel).IsAssignableFrom(panelType))
                    {
                        Debug.LogError($"[UIInstaller] Type '{definition.PanelTypeName}' does not implement IUIPanel.");
                        continue;
                    }

                    _registry.Register(new UIPanelRegistration(
                        PanelId.From(panelType),
                        panelType,
                        definition.AssetPathOrKey,
                        definition.PanelPrefab));
                }
            }

            private static Type ResolvePanelType(string panelTypeName)
            {
                if (string.IsNullOrWhiteSpace(panelTypeName))
                {
                    return null;
                }

                var panelType = Type.GetType(panelTypeName, false);
                if (panelType != null)
                {
                    return panelType;
                }

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (var i = 0; i < assemblies.Length; i++)
                {
                    panelType = assemblies[i].GetType(panelTypeName, false);
                    if (panelType != null)
                    {
                        return panelType;
                    }
                }

                return null;
            }
        }
    }
}
