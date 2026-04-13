using Quest.Core;
using UnityEngine;

namespace Quest.Tutorial
{
    [DisallowMultipleComponent]
    public sealed class TutorialQuestDefinitionAuthoring : MonoBehaviour, IQuestDefinitionSource
    {
        [Header("Quest")]
        [SerializeField] private string _questId = "tutorial.core";
        [SerializeField] private string _questTitle = "Tutorial";
        [SerializeField, TextArea] private string _completedText = "Tutorial complete";

        [Header("Push Step")]
        [SerializeField, Min(1)] private int _pushRequiredCount = 3;
        [SerializeField] private string _pushTitle = "Push objects";
        [SerializeField, TextArea] private string _pushTextFormat = "Push objects: {1}/{2}";
        [SerializeField] private QuestEventFilter _pushFilter = new() { EventId = QuestEventIds.PushPerformed };

        [Header("Pull Step")]
        [SerializeField, Min(1)] private int _pullRequiredCount = 3;
        [SerializeField] private string _pullTitle = "Pull objects";
        [SerializeField, TextArea] private string _pullTextFormat = "Pull objects: {1}/{2}";
        [SerializeField] private QuestEventFilter _pullFilter = new() { EventId = QuestEventIds.PullPerformed };

        [Header("Spell Cast Step")]
        [SerializeField, Min(1)] private int _spellCastRequiredCount = 3;
        [SerializeField] private string _spellCastTitle = "Cast spells";
        [SerializeField, TextArea] private string _spellCastTextFormat = "Cast spells: {1}/{2}";
        [SerializeField] private QuestEventFilter _spellCastFilter = new() { EventId = QuestEventIds.SpellCast };

        [Header("Target Hit Step")]
        [SerializeField, Min(1)] private int _targetHitRequiredCount = 3;
        [SerializeField] private string _targetHitTitle = "Hit targets";
        [SerializeField, TextArea] private string _targetHitTextFormat = "Hit targets: {1}/{2}";
        [SerializeField] private QuestEventFilter _targetHitFilter = new() { EventId = QuestEventIds.TargetHit };

        public QuestDefinitionData CreateDefinition()
        {
            var steps = new[]
            {
                CreateStep("push", _pushTitle, _pushTextFormat, _pushRequiredCount, _pushFilter),
                CreateStep("pull", _pullTitle, _pullTextFormat, _pullRequiredCount, _pullFilter),
                CreateStep("spell_cast", _spellCastTitle, _spellCastTextFormat, _spellCastRequiredCount, _spellCastFilter),
                CreateStep("target_hit", _targetHitTitle, _targetHitTextFormat, _targetHitRequiredCount, _targetHitFilter),
            };

            return new QuestDefinitionData(_questId, _questTitle, _completedText, steps);
        }

        private void OnValidate()
        {
            _pushRequiredCount = Mathf.Max(1, _pushRequiredCount);
            _pullRequiredCount = Mathf.Max(1, _pullRequiredCount);
            _spellCastRequiredCount = Mathf.Max(1, _spellCastRequiredCount);
            _targetHitRequiredCount = Mathf.Max(1, _targetHitRequiredCount);

            _pushFilter ??= new QuestEventFilter { EventId = QuestEventIds.PushPerformed };
            _pullFilter ??= new QuestEventFilter { EventId = QuestEventIds.PullPerformed };
            _spellCastFilter ??= new QuestEventFilter { EventId = QuestEventIds.SpellCast };
            _targetHitFilter ??= new QuestEventFilter { EventId = QuestEventIds.TargetHit };
        }

        private static QuestStepDefinition CreateStep(
            string stepId,
            string title,
            string textFormat,
            int requiredCount,
            QuestEventFilter filter)
        {
            return new QuestStepDefinition
            {
                StepId = stepId,
                Title = title,
                ActiveTextFormat = textFormat,
                CompletedText = "{0} complete",
                RequiredCount = Mathf.Max(1, requiredCount),
                EventFilter = filter != null ? filter.Clone() : new QuestEventFilter(),
                VisibleInUi = true,
            };
        }
    }
}
