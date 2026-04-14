using System;
using UnityEngine;

namespace Quest.Core
{
    [Serializable]
    public sealed class QuestStepDto
    {
        public string StepId = "step";
        public string Title = "Quest Step";
        [TextArea] public string ActiveTextFormat = "{0}: {1}/{2}";
        [TextArea] public string CompletedText = "{0} complete";
        public QuestExpectedSignalDto ExpectedSignal = new();
        public bool VisibleInUi = true;

        public QuestStepDto Clone()
        {
            return new QuestStepDto
            {
                StepId = StepId,
                Title = Title,
                ActiveTextFormat = ActiveTextFormat,
                CompletedText = CompletedText,
                ExpectedSignal = ExpectedSignal != null ? ExpectedSignal.Clone() : new QuestExpectedSignalDto(),
                VisibleInUi = VisibleInUi,
            };
        }

        public void Normalize()
        {
            StepId = string.IsNullOrWhiteSpace(StepId) ? "step" : StepId.Trim();
            Title = string.IsNullOrWhiteSpace(Title) ? StepId : Title.Trim();
            ActiveTextFormat = string.IsNullOrWhiteSpace(ActiveTextFormat) ? "{0}: {1}/{2}" : ActiveTextFormat;
            CompletedText = string.IsNullOrWhiteSpace(CompletedText) ? "{0} complete" : CompletedText;
            ExpectedSignal ??= new QuestExpectedSignalDto();
            ExpectedSignal.Normalize();
        }
    }
}
