using System;
using System.Text;
using Quest.Events;
using TMPro;
using UnityEngine;
using Utils.Events;

namespace UI
{
    [DisallowMultipleComponent]
    public sealed class QuestEventBusPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private TMP_Text _mainLabel;
        [SerializeField] private TMP_Text _questTitleLabel;
        [SerializeField] private TMP_Text _stepLabel;
        [SerializeField] private TMP_Text _progressLabel;
        [SerializeField] private TMP_Text _stateLabel;

        [Header("Formatting")]
        [SerializeField] private string _questTitlePrefix = "Quest: ";
        [SerializeField] private string _stepFormat = "Step {0}/{1}: {2}";
        [SerializeField] private string _progressFormat = "Progress: {0}/{1}";
        [SerializeField] private string _stateActiveText = "Active";
        [SerializeField] private string _stateCompletedText = "Completed";
        [SerializeField] private string _stateIdleText = "No active quest";

        [Header("Behavior")]
        [SerializeField] private bool _hideEmptyLabels = true;

        private EventBus _eventBus;
        private bool _isSubscribed;

        private string _currentQuestId = string.Empty;
        private string _currentQuestTitle = string.Empty;
        private string _currentStepId = string.Empty;
        private string _currentStepTitle = string.Empty;
        private int _currentStepIndex = -1;
        private int _currentStepCount = -1;
        private int _currentProgress = -1;
        private int _currentRequiredProgress = -1;
        private bool _hasActiveQuest;
        private bool _isCompleted;

        private void OnEnable()
        {
            TrySubscribeToEventBus();
            RefreshView();
        }

        private void Update()
        {
            TrySubscribeToEventBus();
        }

        private void OnDisable()
        {
            UnsubscribeFromEventBus();
        }

        private void OnQuestStateChanged(QuestStateChangedEvent eventData)
        {
            if (!eventData.HasActiveQuest && !eventData.IsCompleted)
            {
                ResetViewState();
                RefreshView();
                return;
            }

            ApplySnapshot(
                eventData.QuestId,
                eventData.QuestTitle,
                eventData.ActiveStepId,
                eventData.ActiveStepTitle,
                eventData.ActiveStepIndex,
                eventData.ActiveStepCount,
                eventData.CurrentStepProgress,
                eventData.CurrentStepRequiredProgress,
                eventData.HasActiveQuest,
                eventData.IsCompleted);
        }

        private void OnQuestStepProgressChanged(QuestStepProgressChangedEvent eventData)
        {
            if (!ShouldAcceptQuestEvent(eventData.QuestId))
            {
                return;
            }

            ApplySnapshot(
                eventData.QuestId,
                _currentQuestTitle,
                eventData.ActiveStepId,
                eventData.ActiveStepTitle,
                eventData.ActiveStepIndex,
                eventData.ActiveStepCount,
                eventData.CurrentStepProgress,
                eventData.CurrentStepRequiredProgress,
                true,
                _isCompleted);
        }

        private void OnQuestCompleted(QuestCompletedEvent eventData)
        {
            if (!ShouldAcceptQuestEvent(eventData.QuestId))
            {
                return;
            }

            _currentQuestId = eventData.QuestId ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(eventData.QuestTitle))
            {
                _currentQuestTitle = eventData.QuestTitle;
            }

            _hasActiveQuest = true;
            _isCompleted = true;
            RefreshView();
        }

        private void ApplySnapshot(
            string questId,
            string questTitle,
            string activeStepId,
            string activeStepTitle,
            int activeStepIndex,
            int activeStepCount,
            int currentStepProgress,
            int currentStepRequiredProgress,
            bool hasActiveQuest,
            bool isCompleted)
        {
            _currentQuestId = questId ?? string.Empty;
            _currentQuestTitle = questTitle ?? string.Empty;
            _currentStepId = activeStepId ?? string.Empty;
            _currentStepTitle = activeStepTitle ?? string.Empty;
            _currentStepIndex = activeStepIndex;
            _currentStepCount = activeStepCount;
            _currentProgress = currentStepProgress;
            _currentRequiredProgress = currentStepRequiredProgress;
            _hasActiveQuest = hasActiveQuest;
            _isCompleted = isCompleted;

            RefreshView();
        }

        private void ResetViewState()
        {
            _currentQuestId = string.Empty;
            _currentQuestTitle = string.Empty;
            _currentStepId = string.Empty;
            _currentStepTitle = string.Empty;
            _currentStepIndex = -1;
            _currentStepCount = -1;
            _currentProgress = -1;
            _currentRequiredProgress = -1;
            _hasActiveQuest = false;
            _isCompleted = false;
        }

        private void RefreshView()
        {
            var questTitleText = BuildQuestTitleText();
            var stepText = BuildStepText();
            var progressText = BuildProgressText();
            var stateText = BuildStateText();
            var mainText = BuildMainText(questTitleText, stepText, progressText, stateText);

            SetLabelText(_mainLabel, mainText);
            SetLabelText(_questTitleLabel, questTitleText);
            SetLabelText(_stepLabel, stepText);
            SetLabelText(_progressLabel, progressText);
            SetLabelText(_stateLabel, stateText);
        }

        private string BuildQuestTitleText()
        {
            if (!_hasActiveQuest && !_isCompleted)
            {
                return _stateIdleText;
            }

            if (!string.IsNullOrWhiteSpace(_currentQuestTitle))
            {
                return _currentQuestTitle;
            }

            return !string.IsNullOrWhiteSpace(_currentQuestId)
                ? _currentQuestId
                : (_isCompleted ? _stateCompletedText : _stateIdleText);
        }

        private string BuildStepText()
        {
            if (!_hasActiveQuest && !_isCompleted)
            {
                return string.Empty;
            }

            if (_currentStepIndex < 0 || _currentStepCount <= 0)
            {
                return string.IsNullOrWhiteSpace(_currentStepTitle)
                    ? string.Empty
                    : _currentStepTitle;
            }

            var stepTitle = !string.IsNullOrWhiteSpace(_currentStepTitle)
                ? _currentStepTitle
                : (!string.IsNullOrWhiteSpace(_currentStepId) ? _currentStepId : "Step");

            return string.Format(
                _stepFormat,
                _currentStepIndex + 1,
                _currentStepCount,
                stepTitle);
        }

        private string BuildProgressText()
        {
            if (!_hasActiveQuest && !_isCompleted)
            {
                return string.Empty;
            }

            if (_currentProgress < 0 || _currentRequiredProgress < 0)
            {
                return string.Empty;
            }

            return string.Format(_progressFormat, _currentProgress, _currentRequiredProgress);
        }

        private string BuildStateText()
        {
            if (_isCompleted)
            {
                return _stateCompletedText;
            }

            if (_hasActiveQuest)
            {
                return _stateActiveText;
            }

            return _stateIdleText;
        }

        private string BuildMainText(string questTitleText, string stepText, string progressText, string stateText)
        {
            var builder = new StringBuilder(128);

            AppendLine(builder, string.IsNullOrWhiteSpace(questTitleText) ? string.Empty : $"{_questTitlePrefix}{questTitleText}");
            AppendLine(builder, stepText);
            AppendLine(builder, progressText);
            AppendLine(builder, string.IsNullOrWhiteSpace(stateText) ? string.Empty : $"State: {stateText}");

            return builder.ToString().TrimEnd();
        }

        private static void AppendLine(StringBuilder builder, string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return;
            }

            if (builder.Length > 0)
            {
                builder.AppendLine();
            }

            builder.Append(line);
        }

        private void SetLabelText(TMP_Text label, string text)
        {
            if (label == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                label.text = string.Empty;

                if (_hideEmptyLabels)
                {
                    label.gameObject.SetActive(false);
                }

                return;
            }

            label.gameObject.SetActive(true);
            label.text = text;
        }

        private bool ShouldAcceptQuestEvent(string questId)
        {
            if (string.IsNullOrWhiteSpace(_currentQuestId) || string.IsNullOrWhiteSpace(questId))
            {
                return true;
            }

            return string.Equals(_currentQuestId, questId, StringComparison.Ordinal);
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

            _eventBus.Subscribe<QuestStateChangedEvent>(OnQuestStateChanged);
            _eventBus.Subscribe<QuestStepProgressChangedEvent>(OnQuestStepProgressChanged);
            _eventBus.Subscribe<QuestCompletedEvent>(OnQuestCompleted);
            _isSubscribed = true;
        }

        private void UnsubscribeFromEventBus()
        {
            if (!_isSubscribed || _eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<QuestStateChangedEvent>(OnQuestStateChanged);
            _eventBus.Unsubscribe<QuestStepProgressChangedEvent>(OnQuestStepProgressChanged);
            _eventBus.Unsubscribe<QuestCompletedEvent>(OnQuestCompleted);
            _isSubscribed = false;
            _eventBus = null;
        }
    }
}
