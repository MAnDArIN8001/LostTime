using System;

namespace Quest.Core
{
    public sealed class QuestDefinitionData
    {
        public QuestDefinitionData(
            string questId,
            string title,
            string completedText,
            QuestStepDefinition[] steps)
        {
            QuestId = string.IsNullOrWhiteSpace(questId) ? "quest" : questId.Trim();
            Title = string.IsNullOrWhiteSpace(title) ? QuestId : title.Trim();
            CompletedText = string.IsNullOrWhiteSpace(completedText) ? $"{Title} complete" : completedText.Trim();
            Steps = steps ?? Array.Empty<QuestStepDefinition>();
        }

        public string QuestId { get; }
        public string Title { get; }
        public string CompletedText { get; }
        public QuestStepDefinition[] Steps { get; }
    }
}
