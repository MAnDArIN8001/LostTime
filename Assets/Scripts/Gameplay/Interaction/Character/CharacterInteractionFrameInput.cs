using UnityEngine;

namespace Gameplay.Interaction.Character
{
    public readonly struct CharacterInteractionFrameInput
    {
        public CharacterInteractionFrameInput(
            bool interactionRequested,
            GameObject interactor,
            Vector2 movementInput,
            bool isAimPressed,
            bool castRequestedThisFrame)
        {
            InteractionRequested = interactionRequested;
            Interactor = interactor;
            MovementInput = movementInput;
            IsAimPressed = isAimPressed;
            CastRequestedThisFrame = castRequestedThisFrame;
        }

        public bool InteractionRequested { get; }
        public GameObject Interactor { get; }
        public Vector2 MovementInput { get; }
        public bool IsAimPressed { get; }
        public bool CastRequestedThisFrame { get; }

        public bool HasMovementInput => MovementInput.sqrMagnitude > 0.0001f;
    }
}
