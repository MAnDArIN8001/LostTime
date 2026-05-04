using System;
using System.Collections.Generic;
using Dialogue.Core;

namespace Dialogue.Runtime
{
    public interface IQuestStateQuery
    {
        bool IsQuestCompleted(string questId);
    }

    public sealed class DialogueSession
    {
        private readonly IQuestStateQuery _questStateQuery;

        private readonly DialogueDefinitionData _definition;
        private readonly DialogueHistory _globalHistory;

        private readonly Dictionary<string, DialogueNodeData> _nodesById;
        private readonly Dictionary<string, string> _localChoiceByNode = new(StringComparer.Ordinal);

        private DialogueNodeData _currentNode;

        public DialogueSession(DialogueDefinitionData definition, DialogueHistory globalHistory, IQuestStateQuery questStateQuery)
        {
            _definition = definition ?? new DialogueDefinitionData();
            _globalHistory = globalHistory ?? new DialogueHistory();
            _questStateQuery = questStateQuery;
            _nodesById = BuildNodeIndex(_definition.Nodes);
            Status = DialogueStatus.Idle;
        }

        public DialogueStatus Status { get; private set; }
        public DialogueNodeData CurrentNode => _currentNode;
        public string DialogueId => _definition.DialogueId ?? string.Empty;

        public event Action<DialogueNodeData> StepShown;
        public event Action<string, string> OptionSelected;
        public event Action<string> StepCompleted;
        public event Action Completed;
        public event Action Broken;

        public bool Start()
        {
            if (Status != DialogueStatus.Idle)
            {
                return false;
            }

            var startNode = ResolveNode(_definition.StartNodeId);
            if (startNode == null)
            {
                return false;
            }

            Status = DialogueStatus.Running;
            _currentNode = startNode;
            StepShown?.Invoke(_currentNode);
            return true;
        }

        public bool Continue()
        {
            if (Status != DialogueStatus.Running || _currentNode == null)
            {
                return false;
            }

            if (_currentNode.NodeType == DialogueNodeType.End)
            {
                StepCompleted?.Invoke(_currentNode.NodeId);
                Complete();
                return true;
            }

            if (_currentNode.NodeType != DialogueNodeType.Continue)
            {
                return false;
            }

            var options = GetAvailableOptions(_currentNode);
            if (options.Count == 0)
            {
                StepCompleted?.Invoke(_currentNode.NodeId);
                Complete();
                return true;
            }

            var nextNodeId = options[0].NextNodeId;
            StepCompleted?.Invoke(_currentNode.NodeId);
            return MoveTo(nextNodeId);
        }

        public bool SelectOption(string optionId)
        {
            if (Status != DialogueStatus.Running || _currentNode == null || _currentNode.NodeType != DialogueNodeType.Choice)
            {
                return false;
            }

            var options = GetAvailableOptions(_currentNode);
            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                if (!string.Equals(option.OptionId, optionId, StringComparison.Ordinal))
                {
                    continue;
                }

                _localChoiceByNode[_currentNode.NodeId] = option.OptionId;
                _globalHistory.Entries.Add(new DialogueHistoryEntry(DialogueId, _currentNode.NodeId, option.OptionId));
                OptionSelected?.Invoke(_currentNode.NodeId, option.OptionId);
                StepCompleted?.Invoke(_currentNode.NodeId);

                if (string.IsNullOrWhiteSpace(option.NextNodeId))
                {
                    Complete();
                    return true;
                }

                return MoveTo(option.NextNodeId);
            }

            return false;
        }

        public IReadOnlyList<DialogueOptionData> GetAvailableOptions(DialogueNodeData node)
        {
            var result = new List<DialogueOptionData>();
            if (node == null || node.Options == null)
            {
                return result;
            }

            for (var i = 0; i < node.Options.Count; i++)
            {
                var option = node.Options[i];
                if (option == null)
                {
                    continue;
                }

                if (AreConditionsMet(option.Conditions))
                {
                    result.Add(option);
                }
            }

            return result;
        }

        public void Break()
        {
            if (Status != DialogueStatus.Running)
            {
                return;
            }

            Status = DialogueStatus.Broken;
            Broken?.Invoke();
        }

        private bool MoveTo(string nextNodeId)
        {
            var nextNode = ResolveNode(nextNodeId);
            if (nextNode == null)
            {
                Complete();
                return true;
            }

            _currentNode = nextNode;
            StepShown?.Invoke(_currentNode);
            return true;
        }

        private void Complete()
        {
            if (Status != DialogueStatus.Running)
            {
                return;
            }

            Status = DialogueStatus.Completed;
            if (!string.IsNullOrWhiteSpace(DialogueId))
            {
                _globalHistory.CompletedDialogues.Add(DialogueId);
            }

            Completed?.Invoke();
        }

        private DialogueNodeData ResolveNode(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                return null;
            }

            if (!_nodesById.TryGetValue(nodeId, out var node))
            {
                return null;
            }

            if (!AreConditionsMet(node.EntryConditions))
            {
                return null;
            }

            return node;
        }

        private bool AreConditionsMet(IReadOnlyList<DialogueConditionData> conditions)
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

                if (!EvaluateCondition(condition))
                {
                    return false;
                }
            }

            return true;
        }

        private bool EvaluateCondition(DialogueConditionData condition)
        {
            switch (condition.ConditionType)
            {
                case DialogueConditionType.AlwaysTrue:
                    return true;
                case DialogueConditionType.PreviousChoiceIs:
                    return EvaluatePreviousChoiceCondition(condition);
                case DialogueConditionType.QuestCompleted:
                    return _questStateQuery != null && _questStateQuery.IsQuestCompleted(condition.Key);
                default:
                    return false;
            }
        }

        private bool EvaluatePreviousChoiceCondition(DialogueConditionData condition)
        {
            if (string.IsNullOrWhiteSpace(condition.Key))
            {
                return false;
            }

            if (_localChoiceByNode.TryGetValue(condition.Key, out var localChoice))
            {
                return string.Equals(localChoice, condition.Value, StringComparison.Ordinal);
            }

            for (var i = _globalHistory.Entries.Count - 1; i >= 0; i--)
            {
                var entry = _globalHistory.Entries[i];
                if (!string.Equals(entry.DialogueId, DialogueId, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!string.Equals(entry.NodeId, condition.Key, StringComparison.Ordinal))
                {
                    continue;
                }

                return string.Equals(entry.OptionId, condition.Value, StringComparison.Ordinal);
            }

            return false;
        }

        private static Dictionary<string, DialogueNodeData> BuildNodeIndex(IReadOnlyList<DialogueNodeData> nodes)
        {
            var result = new Dictionary<string, DialogueNodeData>(StringComparer.Ordinal);
            if (nodes == null)
            {
                return result;
            }

            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (node == null || string.IsNullOrWhiteSpace(node.NodeId))
                {
                    continue;
                }

                result[node.NodeId] = node;
            }

            return result;
        }
    }
}
