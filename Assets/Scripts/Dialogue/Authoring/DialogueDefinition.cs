using System;
using System.Collections.Generic;
using Dialogue.Core;
using UnityEngine;

namespace Dialogue.Authoring
{
    [CreateAssetMenu(fileName = "DialogueDefinition", menuName = "LostTime/Dialogue/Dialogue Definition")]
    public sealed class DialogueDefinition : ScriptableObject
    {
        [field: SerializeField] public string DialogueId { get; private set; } = string.Empty;
        [field: SerializeField] public string DisplayName { get; private set; } = string.Empty;
        [field: SerializeField] public string StartNodeId { get; private set; } = string.Empty;
        [SerializeField] private List<DialogueNodeDto> _nodes = new();

        public IReadOnlyList<DialogueNodeDto> Nodes => _nodes;

        public DialogueDefinitionData ToData()
        {
            var result = new DialogueDefinitionData
            {
                DialogueId = string.IsNullOrWhiteSpace(DialogueId) ? name : DialogueId.Trim(),
                DisplayName = DisplayName?.Trim() ?? string.Empty,
                StartNodeId = StartNodeId?.Trim() ?? string.Empty,
                Nodes = new List<DialogueNodeData>(Nodes?.Count ?? 0)
            };

            for (var i = 0; i < (Nodes?.Count ?? 0); i++)
            {
                var sourceNode = Nodes[i] ?? new DialogueNodeDto();
                var node = new DialogueNodeData
                {
                    NodeId = sourceNode.NodeId?.Trim() ?? string.Empty,
                    SpeakerId = sourceNode.SpeakerId?.Trim() ?? string.Empty,
                    SpeakerName = sourceNode.SpeakerName?.Trim() ?? string.Empty,
                    Text = sourceNode.Text ?? string.Empty,
                    NodeType = sourceNode.NodeType,
                    EntryConditions = ToConditionData(sourceNode.EntryConditions),
                    Options = new List<DialogueOptionData>(sourceNode.Options?.Count ?? 0)
                };

                var options = sourceNode.Options;
                if (options == null)
                {
                    result.Nodes.Add(node);
                    continue;
                }

                for (var optionIndex = 0; optionIndex < options.Count; optionIndex++)
                {
                    var sourceOption = options[optionIndex] ?? new DialogueOptionDto();
                    node.Options.Add(new DialogueOptionData
                    {
                        OptionId = sourceOption.OptionId?.Trim() ?? string.Empty,
                        Text = sourceOption.Text ?? string.Empty,
                        NextNodeId = sourceOption.NextNodeId?.Trim() ?? string.Empty,
                        Conditions = ToConditionData(sourceOption.Conditions)
                    });
                }

                result.Nodes.Add(node);
            }

            return result;
        }

        private static List<DialogueConditionData> ToConditionData(IReadOnlyList<DialogueConditionDto> source)
        {
            var conditions = new List<DialogueConditionData>(source?.Count ?? 0);
            if (source == null)
            {
                return conditions;
            }

            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i] ?? new DialogueConditionDto();
                conditions.Add(new DialogueConditionData
                {
                    ConditionType = item.ConditionType,
                    Key = item.Key?.Trim() ?? string.Empty,
                    Value = item.Value?.Trim() ?? string.Empty
                });
            }

            return conditions;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(StartNodeId) && (Nodes?.Count ?? 0) > 0 && Nodes[0] != null)
            {
                StartNodeId = Nodes[0].NodeId;
            }

            ValidateIds();
        }

        private void ValidateIds()
        {
            var data = ToData();
            var issues = DialogueDefinitionValidation.Validate(data);
            for (var i = 0; i < issues.Count; i++)
            {
                var issue = issues[i];
                var message = $"DialogueDefinition '{name}': {issue.Message}";
                if (issue.Severity == DialogueValidationSeverity.Error)
                {
                    Debug.LogError(message, this);
                    continue;
                }

                Debug.LogWarning(message, this);
            }
        }
    }

    [Serializable]
    public sealed class DialogueNodeDto
    {
        [field: SerializeField] public string NodeId { get; private set; } = string.Empty;
        [field: SerializeField] public string SpeakerId { get; private set; } = string.Empty;
        [field: SerializeField] public string SpeakerName { get; private set; } = string.Empty;
        [field: SerializeField, TextArea(2, 8)] public string Text { get; private set; } = string.Empty;
        [field: SerializeField] public DialogueNodeType NodeType { get; private set; } = DialogueNodeType.Continue;
        [SerializeField] private List<DialogueConditionDto> _entryConditions = new();
        [SerializeField] private List<DialogueOptionDto> _options = new();

        public IReadOnlyList<DialogueConditionDto> EntryConditions => _entryConditions;
        public IReadOnlyList<DialogueOptionDto> Options => _options;
    }

    [Serializable]
    public sealed class DialogueOptionDto
    {
        [field: SerializeField] public string OptionId { get; private set; } = string.Empty;
        [field: SerializeField] public string Text { get; private set; } = string.Empty;
        [field: SerializeField] public string NextNodeId { get; private set; } = string.Empty;
        [SerializeField] private List<DialogueConditionDto> _conditions = new();

        public IReadOnlyList<DialogueConditionDto> Conditions => _conditions;
    }

    [Serializable]
    public sealed class DialogueConditionDto
    {
        [field: SerializeField] public DialogueConditionType ConditionType { get; private set; } = DialogueConditionType.AlwaysTrue;
        [field: SerializeField] public string Key { get; private set; } = string.Empty;
        [field: SerializeField] public string Value { get; private set; } = string.Empty;
    }
}
