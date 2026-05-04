using System.Collections.Generic;
using Dialogue.Core;

namespace Dialogue.UI
{
    public sealed class DialogueViewModel
    {
        public DialogueViewModel(
            string speakerName,
            string text,
            DialogueNodeType nodeType,
            IReadOnlyList<DialoguePanel.OptionView> options,
            bool isCancelable)
        {
            SpeakerName = speakerName ?? string.Empty;
            Text = text ?? string.Empty;
            NodeType = nodeType;
            Options = options ?? new List<DialoguePanel.OptionView>();
            IsCancelable = isCancelable;
        }

        public string SpeakerName { get; }
        public string Text { get; }
        public DialogueNodeType NodeType { get; }
        public IReadOnlyList<DialoguePanel.OptionView> Options { get; }
        public bool IsCancelable { get; }
    }
}
