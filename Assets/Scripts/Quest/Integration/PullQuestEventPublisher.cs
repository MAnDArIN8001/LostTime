using Gameplay.Interaction.World;
using Quest.Core;
using UnityEngine;
using Utils.Events;

namespace Quest.Integration
{
    [DisallowMultipleComponent]
    public sealed class PullQuestEventPublisher : MonoBehaviour
    {
        [SerializeField] private PullControllableWorldObject _target;
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private string _sourceId = "pull_object";
        [SerializeField] private string _targetId = "pull_object";
        [SerializeField] private bool _publishOnPress = true;
        [SerializeField] private bool _publishOnControlStarted = true;

        private void OnEnable()
        {
            if (_target == null)
            {
                return;
            }

            _target.PressExecuted += OnPressExecuted;
            _target.ControlStarted += OnControlStarted;
        }

        private void OnDisable()
        {
            if (_target == null)
            {
                return;
            }

            _target.PressExecuted -= OnPressExecuted;
            _target.ControlStarted -= OnControlStarted;
        }

        private void OnPressExecuted(GameObject interactor)
        {
            if (_publishOnPress)
            {
                Publish(interactor);
            }
        }

        private void OnControlStarted(GameObject interactor)
        {
            if (_publishOnControlStarted)
            {
                Publish(interactor);
            }
        }

        private void Publish(GameObject interactor)
        {
            if (!TryResolveEventBus(out var eventBus))
            {
                return;
            }

            eventBus.Publish(new QuestEventData(
                QuestEventIds.PullPerformed,
                _sourceId,
                _targetId,
                1,
                interactor,
                _target != null ? _target.gameObject : null));
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

        private void Reset()
        {
            if (_target == null)
            {
                _target = GetComponent<PullControllableWorldObject>();
            }
        }
    }
}
