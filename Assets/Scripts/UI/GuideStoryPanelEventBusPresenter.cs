using Gameplay.Guide.Core;
using UI.Runtime;
using UnityEngine;
using Utils.Events;
using Zenject;

namespace UI
{
    [DisallowMultipleComponent]
    public sealed class GuideStoryPanelEventBusPresenter : MonoBehaviour
    {
        [SerializeField] private SceneEventBusProvider _eventBusProvider;

        [InjectOptional] private IUIService _uiService;

        private EventBus _eventBus;
        private bool _isSubscribed;

        private void OnEnable()
        {
            TrySubscribeToEventBus();
        }

        private void Update()
        {
            TrySubscribeToEventBus();
        }

        private void OnDisable()
        {
            UnsubscribeFromEventBus();
        }

        private void OnGuideStoryRequested(GuideStoryRequestedEvent eventData)
        {
            if (_uiService == null)
            {
                Debug.LogWarning("[GuideStoryPanelEventBusPresenter] IUIService is not available.");
                return;
            }

            var panel = _uiService.Open<GuideStoryPanel>();
            panel?.SetMetadata(eventData.Metadata);
        }

        private bool TryResolveEventBus(out EventBus eventBus)
        {
            if (_eventBusProvider != null && _eventBusProvider.EventBus != null)
            {
                eventBus = _eventBusProvider.EventBus;
                return true;
            }

            return SceneEventBusProvider.TryGetEventBus(out eventBus);
        }

        private void TrySubscribeToEventBus()
        {
            if (_isSubscribed)
            {
                return;
            }

            if (!TryResolveEventBus(out _eventBus))
            {
                return;
            }

            _eventBus.Subscribe<GuideStoryRequestedEvent>(OnGuideStoryRequested);
            _isSubscribed = true;
        }

        private void UnsubscribeFromEventBus()
        {
            if (!_isSubscribed || _eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<GuideStoryRequestedEvent>(OnGuideStoryRequested);
            _eventBus = null;
            _isSubscribed = false;
        }
    }
}
