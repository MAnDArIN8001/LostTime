using Gameplay.Interaction.Core;

namespace Gameplay.Interaction.Character
{
    public readonly struct CharacterInteractionDiagnosticsSnapshot
    {
        public CharacterInteractionDiagnosticsSnapshot(
            int frame,
            bool interactionRequested,
            bool interactionHandled,
            in InteractionFocusContext focusContext,
            in InteractionIntentResolution resolution,
            in CharacterControlSessionPolicyDecision policyDecision,
            in ControlSessionSnapshot controlSessionSnapshot)
        {
            Frame = frame;
            InteractionRequested = interactionRequested;
            InteractionHandled = interactionHandled;
            FocusContext = focusContext;
            Resolution = resolution;
            PolicyDecision = policyDecision;
            ControlSessionSnapshot = controlSessionSnapshot;
        }

        public int Frame { get; }
        public bool InteractionRequested { get; }
        public bool InteractionHandled { get; }
        public InteractionFocusContext FocusContext { get; }
        public InteractionIntentResolution Resolution { get; }
        public CharacterControlSessionPolicyDecision PolicyDecision { get; }
        public ControlSessionSnapshot ControlSessionSnapshot { get; }
    }
}
