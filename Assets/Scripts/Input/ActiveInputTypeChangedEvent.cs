namespace Gameplay.Input
{
    public readonly struct ActiveInputTypeChangedEvent
    {
        public ActiveInputTypeChangedEvent(ActiveInputType inputType)
        {
            InputType = inputType;
        }

        public ActiveInputType InputType { get; }
    }
}
