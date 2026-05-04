using System;
using System.Collections.Generic;
using Dialogue.Core;

namespace Dialogue.Authoring
{
    public enum DialogueValidationSeverity
    {
        Warning = 0,
        Error = 1
    }

    public readonly struct DialogueValidationIssue
    {
        public DialogueValidationIssue(DialogueValidationSeverity severity, string message)
        {
            Severity = severity;
            Message = message;
        }

        public DialogueValidationSeverity Severity { get; }
        public string Message { get; }
    }

    public static class DialogueDefinitionValidation
    {
        public static IReadOnlyList<DialogueValidationIssue> Validate(DialogueDefinitionData data)
        {
            var issues = new List<DialogueValidationIssue>();
            if (data == null)
            {
                issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, "Dialogue data is null."));
                return issues;
            }

            if (string.IsNullOrWhiteSpace(data.DialogueId))
            {
                issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, "DialogueId is required."));
            }

            var nodes = data.Nodes ?? new List<DialogueNodeData>();
            var nodeIds = new HashSet<string>(StringComparer.Ordinal);
            var referencedNodeIds = new HashSet<string>(StringComparer.Ordinal);

            for (var nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
            {
                var node = nodes[nodeIndex];
                if (node == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(node.NodeId))
                {
                    issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, $"Node at index {nodeIndex} has empty NodeId."));
                    continue;
                }

                if (!nodeIds.Add(node.NodeId))
                {
                    issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, $"Duplicate node id '{node.NodeId}'."));
                }

                if (!ValidateConditionTargets(node.EntryConditions))
                {
                    issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, $"Node '{node.NodeId}' has invalid condition target."));
                }

                var options = node.Options ?? new List<DialogueOptionData>();
                if (node.NodeType == DialogueNodeType.Choice && options.Count == 0)
                {
                    issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, $"Choice node '{node.NodeId}' must contain at least one option."));
                }

                var optionIds = new HashSet<string>(StringComparer.Ordinal);
                for (var optionIndex = 0; optionIndex < options.Count; optionIndex++)
                {
                    var option = options[optionIndex];
                    if (option == null)
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(option.OptionId))
                    {
                        issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, $"Node '{node.NodeId}' option at index {optionIndex} has empty OptionId."));
                    }
                    else if (!optionIds.Add(option.OptionId))
                    {
                        issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, $"Node '{node.NodeId}' has duplicate option id '{option.OptionId}'."));
                    }

                    if (!ValidateConditionTargets(option.Conditions))
                    {
                        issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, $"Node '{node.NodeId}' option '{option.OptionId}' has invalid condition target."));
                    }

                    if (!string.IsNullOrWhiteSpace(option.NextNodeId))
                    {
                        referencedNodeIds.Add(option.NextNodeId);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(data.StartNodeId))
            {
                issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, "StartNodeId is required."));
            }
            else if (!nodeIds.Contains(data.StartNodeId))
            {
                issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, $"StartNodeId '{data.StartNodeId}' does not exist."));
            }

            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (node == null || node.Options == null)
                {
                    continue;
                }

                for (var optionIndex = 0; optionIndex < node.Options.Count; optionIndex++)
                {
                    var option = node.Options[optionIndex];
                    if (option == null || string.IsNullOrWhiteSpace(option.NextNodeId))
                    {
                        continue;
                    }

                    if (!nodeIds.Contains(option.NextNodeId))
                    {
                        issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Error, $"Node '{node.NodeId}' option '{option.OptionId}' points to missing next node '{option.NextNodeId}'."));
                    }
                }
            }

            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (node == null || string.IsNullOrWhiteSpace(node.NodeId))
                {
                    continue;
                }

                if (string.Equals(node.NodeId, data.StartNodeId, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!referencedNodeIds.Contains(node.NodeId))
                {
                    issues.Add(new DialogueValidationIssue(DialogueValidationSeverity.Warning, $"Node '{node.NodeId}' is orphaned (not referenced by any option)."));
                }
            }

            return issues;
        }

        private static bool ValidateConditionTargets(IReadOnlyList<DialogueConditionData> conditions)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return true;
            }

            for (var i = 0; i < conditions.Count; i++)
            {
                var condition = conditions[i];
                if (condition == null)
                {
                    continue;
                }

                switch (condition.ConditionType)
                {
                    case DialogueConditionType.AlwaysTrue:
                        continue;
                    case DialogueConditionType.PreviousChoiceIs:
                        if (string.IsNullOrWhiteSpace(condition.Key) || string.IsNullOrWhiteSpace(condition.Value))
                        {
                            return false;
                        }
                        break;
                    case DialogueConditionType.QuestCompleted:
                        if (string.IsNullOrWhiteSpace(condition.Key))
                        {
                            return false;
                        }
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }
    }
}
