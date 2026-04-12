namespace Gameplay.Interaction.Character
{
    public readonly struct CharacterControlSessionPolicyContext
    {
        public CharacterControlSessionPolicyContext(
            bool hasActiveControlSession,
            bool hasMovementInput,
            bool isAimPressed,
            bool castRequestedThisFrame)
        {
            HasActiveControlSession = hasActiveControlSession;
            HasMovementInput = hasMovementInput;
            IsAimPressed = isAimPressed;
            CastRequestedThisFrame = castRequestedThisFrame;
        }

        public bool HasActiveControlSession { get; }
        public bool HasMovementInput { get; }
        public bool IsAimPressed { get; }
        public bool CastRequestedThisFrame { get; }
    }
}
