using Quest.Core;
using UnityEngine;
using Utils.Events;

namespace Quest.Runtime
{
    [DisallowMultipleComponent]
    public sealed class EventBusQuestRunner : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _definitionSource;
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private bool _autoStartOnEnable = true;

        private QuestSession _session;

        private void OnEnable()
        {
            if (_autoStartOnEnable)
            {
                StartQuest();
            }
        }

        private void OnDisable()
        {
            _session?.Dispose();
            _session = null;
        }

        public void StartQuest()
        {
            _session?.Dispose();
            _session = null;

            if (!TryResolveDefinitionSource(out var definitionSource) || !TryResolveEventBus(out var eventBus))
            {
                return;
            }

            var definition = definitionSource.CreateDefinition();
            if (definition == null || definition.Steps == null || definition.Steps.Length == 0)
            {
                return;
            }

            _session = new QuestSession(eventBus, definition);
            _session.Start();
        }

        private bool TryResolveDefinitionSource(out IQuestDefinitionSource definitionSource)
        {
            definitionSource = _definitionSource as IQuestDefinitionSource;
            return definitionSource != null;
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

        private void OnValidate()
        {
            if (_definitionSource == null || _definitionSource is IQuestDefinitionSource)
            {
                return;
            }

            Debug.LogWarning($"{nameof(EventBusQuestRunner)} on '{name}' requires a component implementing {nameof(IQuestDefinitionSource)}.", this);
            _definitionSource = null;
        }
    }
}
