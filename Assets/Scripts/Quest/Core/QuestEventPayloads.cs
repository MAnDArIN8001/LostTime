namespace Quest.Events
{
    public readonly struct QuestStateChangedEvent
    {
        public QuestStateChangedEvent(
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
            QuestId = questId;
            QuestTitle = questTitle;
            ActiveStepId = activeStepId;
            ActiveStepTitle = activeStepTitle;
            ActiveStepIndex = activeStepIndex;
            ActiveStepCount = activeStepCount;
            CurrentStepProgress = currentStepProgress;
            CurrentStepRequiredProgress = currentStepRequiredProgress;
            HasActiveQuest = hasActiveQuest;
            IsCompleted = isCompleted;
        }

        public string QuestId { get; }
        public string QuestTitle { get; }
        public string ActiveStepId { get; }
        public string ActiveStepTitle { get; }
        public int ActiveStepIndex { get; }
        public int ActiveStepCount { get; }
        public int CurrentStepProgress { get; }
        public int CurrentStepRequiredProgress { get; }
        public bool HasActiveQuest { get; }
        public bool IsCompleted { get; }
    }

    public readonly struct QuestStepProgressChangedEvent
    {
        public QuestStepProgressChangedEvent(
            string questId,
            string activeStepId,
            string activeStepTitle,
            int activeStepIndex,
            int activeStepCount,
            int currentStepProgress,
            int currentStepRequiredProgress)
        {
            QuestId = questId;
            ActiveStepId = activeStepId;
            ActiveStepTitle = activeStepTitle;
            ActiveStepIndex = activeStepIndex;
            ActiveStepCount = activeStepCount;
            CurrentStepProgress = currentStepProgress;
            CurrentStepRequiredProgress = currentStepRequiredProgress;
        }

        public string QuestId { get; }
        public string ActiveStepId { get; }
        public string ActiveStepTitle { get; }
        public int ActiveStepIndex { get; }
        public int ActiveStepCount { get; }
        public int CurrentStepProgress { get; }
        public int CurrentStepRequiredProgress { get; }
    }

    public readonly struct QuestCompletedEvent
    {
        public QuestCompletedEvent(string questId, string questTitle)
        {
            QuestId = questId;
            QuestTitle = questTitle;
        }

        public string QuestId { get; }
        public string QuestTitle { get; }
    }

    public readonly struct QuestStepCompletedEvent
    {
        public QuestStepCompletedEvent(
            string questId,
            string questTitle,
            string stepId,
            string stepTitle,
            int stepIndex,
            int stepCount)
        {
            QuestId = questId;
            QuestTitle = questTitle;
            StepId = stepId;
            StepTitle = stepTitle;
            StepIndex = stepIndex;
            StepCount = stepCount;
        }

        public string QuestId { get; }
        public string QuestTitle { get; }
        public string StepId { get; }
        public string StepTitle { get; }
        public int StepIndex { get; }
        public int StepCount { get; }
    }
}
