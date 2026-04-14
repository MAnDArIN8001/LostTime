using System;
using System.Collections.Generic;

namespace Quest.Core
{
    [Serializable]
    public sealed class QuestDefinitionDto
    {
        public string QuestId = "quest.generic";
        public string Title = "Quest";
        public string CompletedText = "Quest complete";
        public List<QuestStepDto> Steps = new();

        public QuestDefinitionDto Clone()
        {
            var steps = new List<QuestStepDto>(Steps?.Count ?? 0);
            if (Steps != null)
            {
                for (var i = 0; i < Steps.Count; i++)
                {
                    var step = Steps[i];
                    if (step == null)
                    {
                        continue;
                    }

                    steps.Add(step.Clone());
                }
            }

            return new QuestDefinitionDto
            {
                QuestId = QuestId,
                Title = Title,
                CompletedText = CompletedText,
                Steps = steps,
            };
        }

        public void Normalize()
        {
            QuestId = string.IsNullOrWhiteSpace(QuestId) ? "quest.generic" : QuestId.Trim();
            Title = string.IsNullOrWhiteSpace(Title) ? QuestId : Title.Trim();
            CompletedText = string.IsNullOrWhiteSpace(CompletedText) ? $"{Title} complete" : CompletedText.Trim();
            Steps ??= new List<QuestStepDto>();

            for (var i = 0; i < Steps.Count; i++)
            {
                Steps[i] ??= new QuestStepDto();
                Steps[i].Normalize();
            }
        }
    }
}
