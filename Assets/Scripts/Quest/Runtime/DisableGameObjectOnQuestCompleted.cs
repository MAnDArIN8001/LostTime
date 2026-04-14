using System;
using Quest.Events;
using UnityEngine;
using Utils.Events;

namespace Quest.Runtime
{
    [DisallowMultipleComponent]
    public sealed class DisableGameObjectOnQuestCompleted : MonoBehaviour
    {
        [SerializeField] private string _questId = string.Empty;
        [SerializeField] private GameObject _target;
        [SerializeField] private SceneEventBusProvider _eventBusProvider;

        private EventBus _eventBus;
        private bool _isSubscribed;

        private void OnEnable()
        {
            TrySubscribe();
        }

        private void Update()
        {
            TrySubscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnQuestCompleted(QuestCompletedEvent eventData)
        {
            if (!IsMatchingQuestId(eventData.QuestId))
            {
                return;
            }

            DisableTarget();
        }

        private void OnQuestStateChanged(QuestStateChangedEvent eventData)
        {
            if (!eventData.IsCompleted || !IsMatchingQuestId(eventData.QuestId))
            {
                return;
            }

            DisableTarget();
        }

        private bool IsMatchingQuestId(string questId)
        {
            var expectedQuestId = NormalizeQuestId(_questId);
            var actualQuestId = NormalizeQuestId(questId);

            if (string.IsNullOrEmpty(expectedQuestId) || string.IsNullOrEmpty(actualQuestId))
            {
                return false;
            }

            return string.Equals(expectedQuestId, actualQuestId, StringComparison.Ordinal);
        }

        private void DisableTarget()
        {
            var target = _target != null ? _target : gameObject;
            if (target == null || !target.activeSelf)
            {
                return;
            }

            target.SetActive(false);
        }

        private void TrySubscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            if (!TryResolveEventBus(out _eventBus))
            {
                return;
            }

            _eventBus.Subscribe<QuestCompletedEvent>(OnQuestCompleted);
            _eventBus.Subscribe<QuestStateChangedEvent>(OnQuestStateChanged);
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed || _eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<QuestCompletedEvent>(OnQuestCompleted);
            _eventBus.Unsubscribe<QuestStateChangedEvent>(OnQuestStateChanged);
            _eventBus = null;
            _isSubscribed = false;
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

        private static string NormalizeQuestId(string questId)
        {
            return string.IsNullOrWhiteSpace(questId)
                ? string.Empty
                : questId.Trim();
        }
    }
}
