namespace Gameplay.Interaction.Core
{
    public readonly struct InteractionFocusContext
    {
        public InteractionFocusContext(
            in PointerTargetContext pointerContext,
            bool hasMarkable,
            bool hasPressable,
            bool hasInteractable,
            bool hasTakable,
            bool hasControlable)
        {
            PointerContext = pointerContext;
            HasMarkable = hasMarkable;
            HasPressable = hasPressable;
            HasInteractable = hasInteractable;
            HasTakable = hasTakable;
            HasControlable = hasControlable;
        }

        public PointerTargetContext PointerContext { get; }
        public bool HasMarkable { get; }
        public bool HasPressable { get; }
        public bool HasInteractable { get; }
        public bool HasTakable { get; }
        public bool HasControlable { get; }

        public bool HasFocusTarget => PointerContext.HasTarget && (HasMarkable || HasPressable || HasInteractable || HasTakable || HasControlable);
    }
}
