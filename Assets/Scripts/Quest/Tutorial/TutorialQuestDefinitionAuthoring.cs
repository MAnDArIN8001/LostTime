using System;
using System.Collections.Generic;
using Quest.Authoring;
using Quest.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Quest.Tutorial
{
    [RequireComponent(typeof(QuestDefinitionAuthoring))]
    [DisallowMultipleComponent]
    public sealed class TutorialQuestDefinitionAuthoring : MonoBehaviour, IQuestDefinitionSource
    {
        [SerializeField] private QuestDefinitionAuthoring _authoring;
        [SerializeField] private bool _applyPresetOnReset = true;

        [FormerlySerializedAs("_questId")]
        [SerializeField, HideInInspector]
        private string _legacyQuestId = string.Empty;

        [FormerlySerializedAs("_questTitle")]
        [SerializeField, HideInInspector]
        private string _legacyQuestTitle = string.Empty;

        [FormerlySerializedAs("_completedText")]
        [SerializeField, HideInInspector]
        private string _legacyCompletedText = string.Empty;

        [FormerlySerializedAs("_pushRequiredCount")]
        [SerializeField, HideInInspector]
        private int _legacyPushRequiredCount;

        [FormerlySerializedAs("_pushTitle")]
        [SerializeField, HideInInspector]
        private string _legacyPushTitle = string.Empty;

        [FormerlySerializedAs("_pushTextFormat")]
        [SerializeField, HideInInspector]
        private string _legacyPushTextFormat = string.Empty;

        [FormerlySerializedAs("_pushFilter")]
        [SerializeField, HideInInspector]
        private QuestEventFilter _legacyPushFilter;

        [FormerlySerializedAs("_pullRequiredCount")]
        [SerializeField, HideInInspector]
        private int _legacyPullRequiredCount;

        [FormerlySerializedAs("_pullTitle")]
        [SerializeField, HideInInspector]
        private string _legacyPullTitle = string.Empty;

        [FormerlySerializedAs("_pullTextFormat")]
        [SerializeField, HideInInspector]
        private string _legacyPullTextFormat = string.Empty;

        [FormerlySerializedAs("_pullFilter")]
        [SerializeField, HideInInspector]
        private QuestEventFilter _legacyPullFilter;

        [FormerlySerializedAs("_spellCastRequiredCount")]
        [SerializeField, HideInInspector]
        private int _legacySpellCastRequiredCount;

        [FormerlySerializedAs("_spellCastTitle")]
        [SerializeField, HideInInspector]
        private string _legacySpellCastTitle = string.Empty;

        [FormerlySerializedAs("_spellCastTextFormat")]
        [SerializeField, HideInInspector]
        private string _legacySpellCastTextFormat = string.Empty;

        [FormerlySerializedAs("_spellCastFilter")]
        [SerializeField, HideInInspector]
        private QuestEventFilter _legacySpellCastFilter;

        [FormerlySerializedAs("_targetHitRequiredCount")]
        [SerializeField, HideInInspector]
        private int _legacyTargetHitRequiredCount;

        [FormerlySerializedAs("_targetHitTitle")]
        [SerializeField, HideInInspector]
        private string _legacyTargetHitTitle = string.Empty;

        [FormerlySerializedAs("_targetHitTextFormat")]
        [SerializeField, HideInInspector]
        private string _legacyTargetHitTextFormat = string.Empty;

        [FormerlySerializedAs("_targetHitFilter")]
        [SerializeField, HideInInspector]
        private QuestEventFilter _legacyTargetHitFilter;

        public QuestDefinitionData[] CreateDefinitions()
        {
            var definition = CreateDefinition();
            return definition != null ? new[] { definition } : Array.Empty<QuestDefinitionData>();
        }

        public QuestDefinitionData CreateDefinition()
        {
            var authoring = ResolveAuthoring();
            return authoring != null
                ? authoring.CreateDefinition()
                : new QuestDefinitionData("tutorial.core", "Tutorial", "Tutorial complete", Array.Empty<QuestStepDto>());
        }

        [ContextMenu("Apply Tutorial Preset")]
        public void ApplyTutorialPreset()
        {
            var authoring = ResolveAuthoring();
            if (authoring == null)
            {
                return;
            }

            authoring.SetDefinition(CreateTutorialDefinition());
        }

        private void Reset()
        {
            EnsureAuthoringReference();
            if (_applyPresetOnReset)
            {
                ApplyTutorialPreset();
            }
        }

        private void OnValidate()
        {
            EnsureAuthoringReference();
            TryMigrateLegacyTutorialDefinition();
        }

        private QuestDefinitionAuthoring ResolveAuthoring()
        {
            EnsureAuthoringReference();
            return _authoring;
        }

        private void EnsureAuthoringReference()
        {
            if (_authoring != null)
            {
                return;
            }

            _authoring = GetComponent<QuestDefinitionAuthoring>();
        }

        private void TryMigrateLegacyTutorialDefinition()
        {
            var authoring = ResolveAuthoring();
            if (authoring == null)
            {
                return;
            }

            var currentDefinition = authoring.GetDefinition();
            if (currentDefinition.Steps != null && currentDefinition.Steps.Count > 0)
            {
                return;
            }

            if (!HasLegacyTutorialData())
            {
                return;
            }

            authoring.SetDefinition(BuildLegacyTutorialDefinition());
            ClearLegacyTutorialData();
        }

        private bool HasLegacyTutorialData()
        {
            return !string.IsNullOrWhiteSpace(_legacyQuestId)
                || !string.IsNullOrWhiteSpace(_legacyQuestTitle)
                || !string.IsNullOrWhiteSpace(_legacyCompletedText)
                || _legacyPushFilter != null
                || _legacyPullFilter != null
                || _legacySpellCastFilter != null
                || _legacyTargetHitFilter != null
                || _legacyPushRequiredCount > 0
                || _legacyPullRequiredCount > 0
                || _legacySpellCastRequiredCount > 0
                || _legacyTargetHitRequiredCount > 0;
        }

        private QuestDefinitionDto BuildLegacyTutorialDefinition()
        {
            var pushRequired = Mathf.Max(1, _legacyPushRequiredCount <= 0 ? 3 : _legacyPushRequiredCount);
            var pullRequired = Mathf.Max(1, _legacyPullRequiredCount <= 0 ? 3 : _legacyPullRequiredCount);
            var castRequired = Mathf.Max(1, _legacySpellCastRequiredCount <= 0 ? 3 : _legacySpellCastRequiredCount);
            var hitRequired = Mathf.Max(1, _legacyTargetHitRequiredCount <= 0 ? 3 : _legacyTargetHitRequiredCount);

            return new QuestDefinitionDto
            {
                QuestId = string.IsNullOrWhiteSpace(_legacyQuestId) ? "tutorial.core" : _legacyQuestId.Trim(),
                Title = string.IsNullOrWhiteSpace(_legacyQuestTitle) ? "Tutorial" : _legacyQuestTitle.Trim(),
                CompletedText = string.IsNullOrWhiteSpace(_legacyCompletedText) ? "Tutorial complete" : _legacyCompletedText.Trim(),
                Steps = new List<QuestStepDto>
                {
                    CreateLegacyStep("push", _legacyPushTitle, _legacyPushTextFormat, _legacyPushFilter, QuestEventIds.PushPerformed, pushRequired),
                    CreateLegacyStep("pull", _legacyPullTitle, _legacyPullTextFormat, _legacyPullFilter, QuestEventIds.PullPerformed, pullRequired),
                    CreateLegacyStep("spell_cast", _legacySpellCastTitle, _legacySpellCastTextFormat, _legacySpellCastFilter, QuestEventIds.SpellCast, castRequired),
                    CreateLegacyStep("target_hit", _legacyTargetHitTitle, _legacyTargetHitTextFormat, _legacyTargetHitFilter, QuestEventIds.TargetHit, hitRequired),
                },
            };
        }

        private void ClearLegacyTutorialData()
        {
            _legacyQuestId = string.Empty;
            _legacyQuestTitle = string.Empty;
            _legacyCompletedText = string.Empty;
            _legacyPushRequiredCount = 0;
            _legacyPullRequiredCount = 0;
            _legacySpellCastRequiredCount = 0;
            _legacyTargetHitRequiredCount = 0;
            _legacyPushTitle = string.Empty;
            _legacyPullTitle = string.Empty;
            _legacySpellCastTitle = string.Empty;
            _legacyTargetHitTitle = string.Empty;
            _legacyPushTextFormat = string.Empty;
            _legacyPullTextFormat = string.Empty;
            _legacySpellCastTextFormat = string.Empty;
            _legacyTargetHitTextFormat = string.Empty;
            _legacyPushFilter = null;
            _legacyPullFilter = null;
            _legacySpellCastFilter = null;
            _legacyTargetHitFilter = null;
        }

        private static QuestDefinitionDto CreateTutorialDefinition()
        {
            return new QuestDefinitionDto
            {
                QuestId = "tutorial.core",
                Title = "Tutorial",
                CompletedText = "Tutorial complete",
                Steps = new List<QuestStepDto>
                {
                    CreateStep("push", "Push objects", QuestEventIds.PushPerformed),
                    CreateStep("pull", "Pull objects", QuestEventIds.PullPerformed),
                    CreateStep("spell_cast", "Cast spells", QuestEventIds.SpellCast),
                    CreateStep("target_hit", "Hit targets", QuestEventIds.TargetHit),
                },
            };
        }

        private static QuestStepDto CreateStep(string stepId, string title, string eventId)
        {
            return new QuestStepDto
            {
                StepId = stepId,
                Title = title,
                ActiveTextFormat = $"{title}: {{1}}/{{2}}",
                CompletedText = "{0} complete",
                VisibleInUi = true,
                ExpectedSignal = new QuestExpectedSignalDto
                {
                    EventId = eventId,
                    RequiredCount = 3,
                },
            };
        }

        private static QuestStepDto CreateLegacyStep(
            string stepId,
            string title,
            string activeTextFormat,
            QuestEventFilter filter,
            string fallbackEventId,
            int requiredCount)
        {
            return new QuestStepDto
            {
                StepId = stepId,
                Title = string.IsNullOrWhiteSpace(title) ? stepId : title,
                ActiveTextFormat = string.IsNullOrWhiteSpace(activeTextFormat) ? "{0}: {1}/{2}" : activeTextFormat,
                CompletedText = "{0} complete",
                VisibleInUi = true,
                ExpectedSignal = filter != null
                    ? filter.ToExpectedSignal(requiredCount)
                    : new QuestExpectedSignalDto
                    {
                        EventId = fallbackEventId,
                        RequiredCount = requiredCount,
                    },
            };
        }
    }
}
