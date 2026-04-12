namespace Gameplay.Interaction.Core
{
    public readonly struct InteractionHintStateChangedEvent
    {
        public InteractionHintStateChangedEvent(string hintText, bool isVisible)
        {
            HintText = hintText;
            IsVisible = isVisible;
        }

        public string HintText { get; }
        public bool IsVisible { get; }
    }
}
