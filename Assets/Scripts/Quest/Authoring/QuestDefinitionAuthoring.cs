using System.Collections.Generic;
using Quest.Core;
using UnityEngine;

namespace Quest.Authoring
{
    [DisallowMultipleComponent]
    public sealed class QuestDefinitionAuthoring : MonoBehaviour, IQuestDefinitionSource
    {
        [SerializeField] private string _questId = "quest.generic";
        [SerializeField] private string _questTitle = "Quest";
        [SerializeField, TextArea] private string _completedText = "Quest complete";
        [SerializeField] private List<QuestStepDefinition> _steps = new();

        public QuestDefinitionData CreateDefinition()
        {
            var runtimeSteps = new List<QuestStepDefinition>(_steps.Count);
            for (var i = 0; i < _steps.Count; i++)
            {
                var step = _steps[i];
                if (step == null)
                {
                    continue;
                }

                runtimeSteps.Add(step.Clone());
            }

            return new QuestDefinitionData(_questId, _questTitle, _completedText, runtimeSteps.ToArray());
        }

        private void OnValidate()
        {
            for (var i = 0; i < _steps.Count; i++)
            {
                var step = _steps[i];
                if (step == null)
                {
                    continue;
                }

                step.RequiredCount = Mathf.Max(1, step.RequiredCount);
                if (step.EventFilter == null)
                {
                    step.EventFilter = new QuestEventFilter();
                }
            }
        }
    }
}
