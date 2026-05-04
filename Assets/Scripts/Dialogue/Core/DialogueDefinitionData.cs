using System;
using System.Collections.Generic;

namespace Dialogue.Core
{
    [Serializable]
    public sealed class DialogueDefinitionData
    {
        public string DialogueId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string StartNodeId { get; set; } = string.Empty;
        public List<DialogueNodeData> Nodes { get; set; } = new();
    }

    [Serializable]
    public sealed class DialogueNodeData
    {
        public string NodeId { get; set; } = string.Empty;
        public string SpeakerId { get; set; } = string.Empty;
        public string SpeakerName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DialogueNodeType NodeType { get; set; } = DialogueNodeType.Continue;
        public List<DialogueConditionData> EntryConditions { get; set; } = new();
        public List<DialogueOptionData> Options { get; set; } = new();
    }

    [Serializable]
    public sealed class DialogueOptionData
    {
        public string OptionId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string NextNodeId { get; set; } = string.Empty;
        public List<DialogueConditionData> Conditions { get; set; } = new();
    }

    [Serializable]
    public sealed class DialogueConditionData
    {
        public DialogueConditionType ConditionType { get; set; } = DialogueConditionType.AlwaysTrue;
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public enum DialogueConditionType
    {
        AlwaysTrue = 0,
        PreviousChoiceIs = 1,
        QuestCompleted = 2
    }
}
