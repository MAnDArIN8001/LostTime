using System;
using Quest.Core;
using Quest.Events;
using Utils.Events;

namespace Quest.Runtime
{
    public sealed class QuestSession : IDisposable
    {
        private readonly EventBus _eventBus;
        private readonly QuestDefinitionData _definition;
        private int _activeStepIndex;
        private int _activeStepProgress;
        private bool _isStarted;
        private bool _isCompleted;

        public event Action<string> Completed;
        public event Action<string, string> StepCompleted;

        public QuestSession(EventBus eventBus, QuestDefinitionData definition)
        {
            _eventBus = eventBus;
            _definition = definition;
            _activeStepIndex = -1;
            _activeStepProgress = 0;
        }

        public void Start()
        {
            if (_isStarted || _eventBus == null || _definition == null || _definition.Steps.Length == 0)
            {
                return;
            }

            _isStarted = true;
            _isCompleted = false;
            _activeStepIndex = 0;
            _activeStepProgress = 0;

            _eventBus.Subscribe<QuestEventData>(OnQuestEvent);
            PublishStateChanged();
        }

        public void PublishCurrentState()
        {
            if (!_isStarted || _eventBus == null)
            {
                return;
            }

            PublishStateChanged();
        }

        public void Dispose()
        {
            if (!_isStarted || _eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<QuestEventData>(OnQuestEvent);
            _isStarted = false;
        }

        private void OnQuestEvent(QuestEventData eventData)
        {
            if (!_isStarted || _isCompleted || _activeStepIndex < 0 || _activeStepIndex >= _definition.Steps.Length)
            {
                return;
            }

            var step = _definition.Steps[_activeStepIndex];
            var expectedSignal = step?.ExpectedSignal;
            if (step == null || expectedSignal == null || !expectedSignal.Matches(eventData))
            {
                return;
            }

            var requiredCount = Math.Max(1, expectedSignal.RequiredCount);
            _activeStepProgress = Math.Min(requiredCount, _activeStepProgress + Math.Max(1, eventData.CountDelta));
            PublishProgressChanged();

            if (_activeStepProgress < requiredCount)
            {
                return;
            }

            var completedStep = step;
            var completedStepIndex = _activeStepIndex;
            PublishStepCompleted(completedStep, completedStepIndex);

            if (_activeStepIndex >= _definition.Steps.Length - 1)
            {
                _isCompleted = true;
                Completed?.Invoke(_definition.QuestId);
                _eventBus.Publish(new QuestCompletedEvent(_definition.QuestId, _definition.Title));
                PublishStateChanged();
                Dispose();
                return;
            }

            _activeStepIndex++;
            _activeStepProgress = 0;
            PublishStateChanged();
        }

        private void PublishStateChanged()
        {
            _eventBus.Publish(new QuestStateChangedEvent(
                _definition.QuestId,
                _definition.Title,
                ActiveStep?.StepId ?? string.Empty,
                ActiveStep?.Title ?? string.Empty,
                _activeStepIndex,
                _definition.Steps.Length,
                _activeStepProgress,
                ActiveStep?.ExpectedSignal?.RequiredCount ?? 0,
                !_isCompleted && ActiveStep != null,
                _isCompleted));
        }

        private void PublishProgressChanged()
        {
            _eventBus.Publish(new QuestStepProgressChangedEvent(
                _definition.QuestId,
                ActiveStep?.StepId ?? string.Empty,
                ActiveStep?.Title ?? string.Empty,
                _activeStepIndex,
                _definition.Steps.Length,
                _activeStepProgress,
                ActiveStep?.ExpectedSignal?.RequiredCount ?? 0));
        }

        private void PublishStepCompleted(QuestStepDto completedStep, int completedStepIndex)
        {
            var stepId = completedStep?.StepId ?? string.Empty;
            var stepTitle = completedStep?.Title ?? string.Empty;

            StepCompleted?.Invoke(_definition.QuestId, stepId);
            _eventBus.Publish(new QuestStepCompletedEvent(
                _definition.QuestId,
                _definition.Title,
                stepId,
                stepTitle,
                completedStepIndex,
                _definition.Steps.Length));
        }

        private QuestStepDto ActiveStep =>
            _activeStepIndex >= 0 && _activeStepIndex < _definition.Steps.Length
                ? _definition.Steps[_activeStepIndex]
                : null;
    }
}
