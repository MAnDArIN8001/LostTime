namespace Dialogue.Runtime
{
    public readonly struct DialogueStartedEvent
    {
        public readonly string DialogueId;
        public readonly string NodeId;

        public DialogueStartedEvent(string dialogueId, string nodeId)
        {
            DialogueId = dialogueId;
            NodeId = nodeId;
        }
    }

    public readonly struct DialogueStepShownEvent
    {
        public readonly string DialogueId;
        public readonly string NodeId;

        public DialogueStepShownEvent(string dialogueId, string nodeId)
        {
            DialogueId = dialogueId;
            NodeId = nodeId;
        }
    }

    public readonly struct DialogueOptionSelectedEvent
    {
        public readonly string DialogueId;
        public readonly string NodeId;
        public readonly string OptionId;

        public DialogueOptionSelectedEvent(string dialogueId, string nodeId, string optionId)
        {
            DialogueId = dialogueId;
            NodeId = nodeId;
            OptionId = optionId;
        }
    }

    public readonly struct DialogueStepCompletedEvent
    {
        public readonly string DialogueId;
        public readonly string NodeId;

        public DialogueStepCompletedEvent(string dialogueId, string nodeId)
        {
            DialogueId = dialogueId;
            NodeId = nodeId;
        }
    }

    public readonly struct DialogueCompletedEvent
    {
        public readonly string DialogueId;

        public DialogueCompletedEvent(string dialogueId)
        {
            DialogueId = dialogueId;
        }
    }

    public readonly struct DialogueBreakEvent
    {
        public readonly string DialogueId;

        public DialogueBreakEvent(string dialogueId)
        {
            DialogueId = dialogueId;
        }
    }
}
