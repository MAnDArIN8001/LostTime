using Gameplay.Interaction.Core;
using Loot.Systems;
using UnityEngine;
using Utils.Events;

namespace Gameplay.Interaction.Character
{
    [DisallowMultipleComponent]
    public sealed class InteractionHintEventPublisher : MonoBehaviour
    {
        [SerializeField] private InteractionController _interactionController;
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private bool _publishCurrentHintOnEnable = true;

        private string _lastPublishedHint = string.Empty;
        private string _pendingHint = string.Empty;

        private void OnEnable()
        {
            if (_interactionController == null)
            {
                return;
            }

            _interactionController.FocusHintChanged += OnFocusHintChanged;

            if (_publishCurrentHintOnEnable)
            {
                PublishHint(_interactionController.CurrentInteractHint);
            }
        }

        private void Update()
        {
            TryPublishPending();
        }

        private void OnDisable()
        {
            if (_interactionController != null)
            {
                _interactionController.FocusHintChanged -= OnFocusHintChanged;
            }

            PublishHint(string.Empty);
        }

        private void OnFocusHintChanged(string hint)
        {
            PublishHint(hint);
        }

        private void PublishHint(string rawHint)
        {
            _pendingHint = string.IsNullOrWhiteSpace(rawHint)
                ? string.Empty
                : rawHint.Trim();

            TryPublishPending();
        }

        private void TryPublishPending()
        {
            if (_lastPublishedHint == _pendingHint)
            {
                return;
            }

            if (TryResolveEventBus(out var eventBus))
            {
                eventBus.Publish(new InteractionHintStateChangedEvent(
                    _pendingHint,
                    !string.IsNullOrEmpty(_pendingHint)));
                _lastPublishedHint = _pendingHint;
            }
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
    }
}
