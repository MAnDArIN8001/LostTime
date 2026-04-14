using System.Collections.Generic;
using Quest.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Quest.Authoring
{
    [DisallowMultipleComponent]
    public sealed class QuestDefinitionAuthoring : MonoBehaviour, IQuestDefinitionSource
    {
        [SerializeField] private List<QuestDefinitionDto> _definitions = new();

        [FormerlySerializedAs("_definition")]
        [SerializeField, HideInInspector]
        private QuestDefinitionDto _legacyDefinition = new();

        [FormerlySerializedAs("_questId")]
        [SerializeField, HideInInspector]
        private string _legacyQuestId = string.Empty;

        [FormerlySerializedAs("_questTitle")]
        [SerializeField, HideInInspector]
        private string _legacyQuestTitle = string.Empty;

        [FormerlySerializedAs("_completedText")]
        [SerializeField, HideInInspector, TextArea]
        private string _legacyCompletedText = string.Empty;

        [FormerlySerializedAs("_steps")]
        [SerializeField, HideInInspector]
        private List<QuestStepDefinition> _legacySteps = new();

        public QuestDefinitionData[] CreateDefinitions()
        {
            NormalizeDefinitions();

            var runtimeDefinitions = new List<QuestDefinitionData>(_definitions.Count);
            for (var i = 0; i < _definitions.Count; i++)
            {
                var definition = _definitions[i];
                if (definition == null)
                {
                    continue;
                }

                runtimeDefinitions.Add(CreateRuntimeDefinition(definition));
            }

            return runtimeDefinitions.ToArray();
        }

        public QuestDefinitionData CreateDefinition()
        {
            var definitions = CreateDefinitions();
            return definitions.Length > 0 ? definitions[0] : null;
        }

        public void SetDefinition(QuestDefinitionDto definitionDto)
        {
            _definitions = new List<QuestDefinitionDto>
            {
                definitionDto != null ? definitionDto.Clone() : new QuestDefinitionDto(),
            };
            NormalizeDefinitions();
        }

        public void SetDefinitions(List<QuestDefinitionDto> definitions)
        {
            _definitions = new List<QuestDefinitionDto>(definitions?.Count ?? 0);
            if (definitions != null)
            {
                for (var i = 0; i < definitions.Count; i++)
                {
                    _definitions.Add(definitions[i]?.Clone() ?? new QuestDefinitionDto());
                }
            }

            NormalizeDefinitions();
        }

        public QuestDefinitionDto GetDefinition()
        {
            NormalizeDefinitions();
            return _definitions.Count > 0 && _definitions[0] != null
                ? _definitions[0].Clone()
                : new QuestDefinitionDto();
        }

        public List<QuestDefinitionDto> GetDefinitions()
        {
            NormalizeDefinitions();
            var result = new List<QuestDefinitionDto>(_definitions.Count);
            for (var i = 0; i < _definitions.Count; i++)
            {
                result.Add(_definitions[i]?.Clone() ?? new QuestDefinitionDto());
            }

            return result;
        }

        private void Reset()
        {
            if (_definitions == null)
            {
                _definitions = new List<QuestDefinitionDto>();
            }

            NormalizeDefinitions();
        }

        private void OnValidate()
        {
            NormalizeDefinitions();
        }

        private void NormalizeDefinitions()
        {
            _definitions ??= new List<QuestDefinitionDto>();

            if (ShouldMigrateLegacyData())
            {
                _definitions = new List<QuestDefinitionDto>
                {
                    BuildDefinitionFromLegacyData(),
                };

                _legacySteps.Clear();
            }

            for (var i = 0; i < _definitions.Count; i++)
            {
                _definitions[i] ??= new QuestDefinitionDto();
                _definitions[i].Normalize();
            }
        }

        private bool ShouldMigrateLegacyData()
        {
            var hasCurrentDefinitions = _definitions != null && _definitions.Count > 0;
            if (hasCurrentDefinitions)
            {
                return false;
            }

            if (HasLegacyDefinitionData())
            {
                return true;
            }

            if (_legacySteps != null && _legacySteps.Count > 0)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(_legacyQuestId)
                || !string.IsNullOrWhiteSpace(_legacyQuestTitle)
                || !string.IsNullOrWhiteSpace(_legacyCompletedText);
        }

        private bool HasLegacyDefinitionData()
        {
            if (_legacyDefinition == null)
            {
                return false;
            }

            return (_legacyDefinition.Steps != null && _legacyDefinition.Steps.Count > 0)
                || !string.IsNullOrWhiteSpace(_legacyDefinition.QuestId) && !string.Equals(_legacyDefinition.QuestId.Trim(), "quest.generic")
                || !string.IsNullOrWhiteSpace(_legacyDefinition.Title) && !string.Equals(_legacyDefinition.Title.Trim(), "Quest")
                || !string.IsNullOrWhiteSpace(_legacyDefinition.CompletedText) && !string.Equals(_legacyDefinition.CompletedText.Trim(), "Quest complete");
        }

        private QuestDefinitionDto BuildDefinitionFromLegacyData()
        {
            if (HasLegacyDefinitionData())
            {
                var migratedLegacy = _legacyDefinition.Clone();
                migratedLegacy.Normalize();
                return migratedLegacy;
            }

            var steps = new List<QuestStepDto>(_legacySteps?.Count ?? 0);
            if (_legacySteps != null)
            {
                for (var i = 0; i < _legacySteps.Count; i++)
                {
                    var legacyStep = _legacySteps[i];
                    if (legacyStep == null)
                    {
                        continue;
                    }

                    steps.Add(legacyStep.ToDto());
                }
            }

            return new QuestDefinitionDto
            {
                QuestId = string.IsNullOrWhiteSpace(_legacyQuestId) ? "quest.generic" : _legacyQuestId,
                Title = string.IsNullOrWhiteSpace(_legacyQuestTitle) ? "Quest" : _legacyQuestTitle,
                CompletedText = string.IsNullOrWhiteSpace(_legacyCompletedText) ? "Quest complete" : _legacyCompletedText,
                Steps = steps,
            };
        }

        private static QuestDefinitionData CreateRuntimeDefinition(QuestDefinitionDto definitionDto)
        {
            var runtimeSteps = new List<QuestStepDto>(definitionDto.Steps.Count);
            for (var i = 0; i < definitionDto.Steps.Count; i++)
            {
                var step = definitionDto.Steps[i];
                if (step == null)
                {
                    continue;
                }

                runtimeSteps.Add(step.Clone());
            }

            return new QuestDefinitionData(definitionDto.QuestId, definitionDto.Title, definitionDto.CompletedText, runtimeSteps.ToArray());
        }
    }
}
