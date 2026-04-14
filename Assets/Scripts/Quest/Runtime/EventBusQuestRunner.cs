using System;
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
        [SerializeField, Min(0)] private int _initialStateRepublishFrames = 3;
        [SerializeField] private string _questId = string.Empty;

        private QuestSession _session;
        private bool _pendingAutoStart;
        private int _pendingStateRepublishFrames;

        public event Action<string> QuestCompleted;
        public event Action<string, string> QuestStepCompleted;

        private void OnEnable()
        {
            if (_autoStartOnEnable)
            {
                _pendingAutoStart = true;
            }
        }

        private void Start()
        {
            TryAutoStart();
        }

        private void Update()
        {
            TryAutoStart();
            TryRepublishInitialState();
        }

        private void OnDisable()
        {
            DetachSession();
            _pendingAutoStart = false;
            _pendingStateRepublishFrames = 0;
        }

        public void StartQuest()
        {
            TryStartQuest();
        }

        private void TryAutoStart()
        {
            if (!_pendingAutoStart || _session != null)
            {
                return;
            }

            _pendingAutoStart = !TryStartQuest();
        }

        private bool TryStartQuest()
        {
            DetachSession();

            if (!TryResolveDefinitionSource(out var definitionSource) || !TryResolveEventBus(out var eventBus))
            {
                return false;
            }

            var definition = ResolveDefinition(definitionSource);
            if (definition == null || definition.Steps == null || definition.Steps.Length == 0)
            {
                return false;
            }

            _session = new QuestSession(eventBus, definition);
            _session.Completed += OnQuestCompleted;
            _session.StepCompleted += OnQuestStepCompleted;
            _session.Start();
            _pendingStateRepublishFrames = _initialStateRepublishFrames;
            return true;
        }

        private void OnQuestCompleted(string questId)
        {
            QuestCompleted?.Invoke(questId);
        }

        private void OnQuestStepCompleted(string questId, string stepId)
        {
            QuestStepCompleted?.Invoke(questId, stepId);
        }

        private void DetachSession()
        {
            if (_session == null)
            {
                return;
            }

            _session.Completed -= OnQuestCompleted;
            _session.StepCompleted -= OnQuestStepCompleted;
            _session.Dispose();
            _session = null;
        }

        private void TryRepublishInitialState()
        {
            if (_session == null || _pendingStateRepublishFrames <= 0)
            {
                return;
            }

            _session.PublishCurrentState();
            _pendingStateRepublishFrames--;
        }

        private bool TryResolveDefinitionSource(out IQuestDefinitionSource definitionSource)
        {
            definitionSource = _definitionSource as IQuestDefinitionSource;
            return definitionSource != null;
        }

        private QuestDefinitionData ResolveDefinition(IQuestDefinitionSource definitionSource)
        {
            var definitions = definitionSource.CreateDefinitions();
            if (definitions == null || definitions.Length == 0)
            {
                return definitionSource.CreateDefinition();
            }

            var desiredQuestId = NormalizeQuestId(_questId);
            if (string.IsNullOrEmpty(desiredQuestId))
            {
                return definitions[0];
            }

            for (var i = 0; i < definitions.Length; i++)
            {
                var definition = definitions[i];
                if (definition == null)
                {
                    continue;
                }

                if (string.Equals(NormalizeQuestId(definition.QuestId), desiredQuestId, StringComparison.Ordinal))
                {
                    return definition;
                }
            }

            return definitions[0];
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

        private static string NormalizeQuestId(string questId)
        {
            return string.IsNullOrWhiteSpace(questId)
                ? string.Empty
                : questId.Trim();
        }
    }
}
