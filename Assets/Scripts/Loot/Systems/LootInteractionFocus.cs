using Gameplay.Interaction.Core;

namespace Loot.Systems
{
    public readonly struct LootInteractionFocus
    {
        public LootInteractionFocus(
            IMarkable markable,
            IPressable pressable,
            IInteractable interactable,
            ITakable takable,
            IControlable controlable,
            in PointerTargetContext pointerContext)
        {
            Markable = markable;
            Pressable = pressable;
            Interactable = interactable;
            Takable = takable;
            Controlable = controlable;
            Context = new InteractionFocusContext(
                pointerContext,
                markable != null,
                pressable != null,
                interactable != null,
                takable != null,
                controlable != null);
        }

        public IMarkable Markable { get; }
        public IPressable Pressable { get; }
        public IInteractable Interactable { get; }
        public ITakable Takable { get; }
        public IControlable Controlable { get; }
        public InteractionFocusContext Context { get; }

        public bool HasAnyTarget => Markable != null || Pressable != null || Interactable != null || Takable != null || Controlable != null;

        public static LootInteractionFocus Empty => default;
    }
}
