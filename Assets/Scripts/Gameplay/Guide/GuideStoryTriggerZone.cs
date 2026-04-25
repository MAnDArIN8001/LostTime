using Gameplay.Guide.Core;
using UnityEngine;
using Utils.Events;

namespace Gameplay.Guide
{
    [DisallowMultipleComponent]
    public sealed class GuideStoryTriggerZone : MonoBehaviour
    {
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private GuideStoryMetadata _metadata;
        [SerializeField] private bool _triggerOnlyOnce = true;

        private bool _isTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggerOnlyOnce && _isTriggered)
            {
                return;
            }

            if (other == null || other.GetComponentInParent<Character.Character>() == null)
            {
                return;
            }

            if (!TryResolveEventBus(out var eventBus))
            {
                return;
            }

            eventBus.Publish(new GuideStoryRequestedEvent(_metadata));
            _isTriggered = true;
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
