using System.Collections.Generic;
using NUnit.Framework;
using UI.Runtime;

namespace Tests.UIRuntime
{
    public sealed class UIServiceTests
    {
        [Test]
        public void CloseTop_ClosesLastOpenedFirst()
        {
            var factory = new FakePanelFactory();
            var service = new UIService(factory);

            service.Open<FirstPanel>();
            service.Open<SecondPanel>();

            var closeTopResult = service.CloseTop();

            Assert.That(closeTopResult, Is.True);
            Assert.That(service.IsOpen<SecondPanel>(), Is.False);
            Assert.That(service.IsOpen<FirstPanel>(), Is.True);
        }

        [Test]
        public void CloseAll_ClosesAllPanels()
        {
            var factory = new FakePanelFactory();
            var service = new UIService(factory);

            service.Open<FirstPanel>();
            service.Open<SecondPanel>();

            service.CloseAll();

            Assert.That(service.IsOpen<FirstPanel>(), Is.False);
            Assert.That(service.IsOpen<SecondPanel>(), Is.False);
        }

        [Test]
        public void KeepAlivePanel_IsReusedBetweenOpenClose()
        {
            var factory = new FakePanelFactory();
            factory.RegisterPanel<KeepAlivePanel>(UICachePolicy.KeepAlive);
            var service = new UIService(factory);

            var firstOpen = service.Open<KeepAlivePanel>();
            service.Close<KeepAlivePanel>();
            var secondOpen = service.Open<KeepAlivePanel>();

            Assert.That(firstOpen, Is.Not.Null);
            Assert.That(secondOpen, Is.SameAs(firstOpen));
            Assert.That(factory.CreateCallCountByPanelId[PanelId.From<KeepAlivePanel>()], Is.EqualTo(1));
        }

        [Test]
        public void DestroyOnClose_ReleasesFactoryInstance()
        {
            var factory = new FakePanelFactory();
            factory.RegisterPanel<FirstPanel>(UICachePolicy.DestroyOnClose);
            var service = new UIService(factory);

            service.Open<FirstPanel>();
            service.Close<FirstPanel>();

            Assert.That(factory.ReleaseCalls, Has.Member(PanelId.From<FirstPanel>()));
        }

        [Test]
        public void Dispose_ClosesOpenAndReleasesKeepAlive()
        {
            var factory = new FakePanelFactory();
            factory.RegisterPanel<FirstPanel>(UICachePolicy.DestroyOnClose);
            factory.RegisterPanel<KeepAlivePanel>(UICachePolicy.KeepAlive);
            var service = new UIService(factory);

            service.Open<FirstPanel>();
            service.Open<KeepAlivePanel>();
            service.Close<KeepAlivePanel>();

            service.Dispose();

            Assert.That(factory.ReleaseCalls, Has.Member(PanelId.From<FirstPanel>()));
            Assert.That(factory.ReleaseCalls, Has.Member(PanelId.From<KeepAlivePanel>()));
            Assert.That(service.CloseTop(), Is.False);
        }

        private sealed class FakePanelFactory : IUIPanelFactory
        {
            private readonly Dictionary<PanelId, IUIPanel> _createdById = new();
            private readonly Dictionary<PanelId, UICachePolicy> _cachePolicyById = new();

            public Dictionary<PanelId, int> CreateCallCountByPanelId { get; } = new();
            public List<PanelId> ReleaseCalls { get; } = new();

            public TPanel Create<TPanel>() where TPanel : class, IUIPanel
            {
                return Create(PanelId.From<TPanel>()) as TPanel;
            }

            public IUIPanel Create(PanelId panelId)
            {
                if (_createdById.TryGetValue(panelId, out var existingPanel))
                {
                    return existingPanel;
                }

                if (!CreateCallCountByPanelId.ContainsKey(panelId))
                {
                    CreateCallCountByPanelId[panelId] = 0;
                }

                CreateCallCountByPanelId[panelId]++;
                var panel = new FakePanel(ResolveCachePolicy(panelId), panelId);
                _createdById[panelId] = panel;
                return panel;
            }

            public bool IsCreated(PanelId panelId)
            {
                return _createdById.ContainsKey(panelId);
            }

            public bool TryGetCreated(PanelId panelId, out IUIPanel panel)
            {
                return _createdById.TryGetValue(panelId, out panel);
            }

            public void Release(PanelId panelId)
            {
                ReleaseCalls.Add(panelId);
                _createdById.Remove(panelId);
            }

            public void ReleaseAll()
            {
                var keys = new List<PanelId>(_createdById.Keys);
                for (var i = 0; i < keys.Count; i++)
                {
                    Release(keys[i]);
                }
            }

            public void RegisterPanel<TPanel>(UICachePolicy policy) where TPanel : class, IUIPanel
            {
                _cachePolicyById[PanelId.From<TPanel>()] = policy;
            }

            private UICachePolicy ResolveCachePolicy(PanelId panelId)
            {
                if (_cachePolicyById.TryGetValue(panelId, out var policy))
                {
                    return policy;
                }

                return UICachePolicy.DestroyOnClose;
            }
        }

        private sealed class FakePanel : IUIPanel
        {
            public PanelId Id { get; private set; }
            public UIPanelConfig Config { get; private set; }
            public bool IsVisible { get; private set; }

            public FakePanel(UICachePolicy cachePolicy, PanelId id)
            {
                Id = id;
                Config = new UIPanelConfig(false, cachePolicy);
            }

            public void Show()
            {
                IsVisible = true;
            }

            public void Hide()
            {
                IsVisible = false;
            }

            public void Close(UIPanelCloseReason reason)
            {
                IsVisible = false;
            }
        }

        private sealed class FirstPanel : IUIPanel
        {
            public PanelId Id { get; } = PanelId.From<FirstPanel>();
            public UIPanelConfig Config { get; } = UIPanelConfig.Default;
            public bool IsVisible { get; private set; }
            public void Show() => IsVisible = true;
            public void Hide() => IsVisible = false;
            public void Close(UIPanelCloseReason reason) => IsVisible = false;
        }

        private sealed class SecondPanel : IUIPanel
        {
            public PanelId Id { get; } = PanelId.From<SecondPanel>();
            public UIPanelConfig Config { get; } = UIPanelConfig.Default;
            public bool IsVisible { get; private set; }
            public void Show() => IsVisible = true;
            public void Hide() => IsVisible = false;
            public void Close(UIPanelCloseReason reason) => IsVisible = false;
        }

        private sealed class KeepAlivePanel : IUIPanel
        {
            public PanelId Id { get; } = PanelId.From<KeepAlivePanel>();
            public UIPanelConfig Config { get; } = UIPanelConfig.NonModalKeepAlive;
            public bool IsVisible { get; private set; }
            public void Show() => IsVisible = true;
            public void Hide() => IsVisible = false;
            public void Close(UIPanelCloseReason reason) => IsVisible = false;
        }
    }
}
