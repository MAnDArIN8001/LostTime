using System;
using UnityEngine;

namespace Quest.Core
{
    [Serializable]
    public sealed class QuestStepDefinition
    {
        public string StepId = "step";
        public string Title = "Quest Step";
        [TextArea] public string ActiveTextFormat = "{0}: {1}/{2}";
        [TextArea] public string CompletedText = "{0} complete";
        public QuestEventFilter EventFilter = new();
        [Min(1)] public int RequiredCount = 1;
        public bool VisibleInUi = true;

        public QuestStepDefinition Clone()
        {
            return new QuestStepDefinition
            {
                StepId = StepId,
                Title = Title,
                ActiveTextFormat = ActiveTextFormat,
                CompletedText = CompletedText,
                EventFilter = EventFilter != null ? EventFilter.Clone() : new QuestEventFilter(),
                RequiredCount = Mathf.Max(1, RequiredCount),
                VisibleInUi = VisibleInUi,
            };
        }
    }
}
