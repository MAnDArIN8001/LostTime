namespace Gameplay.Interaction.Character
{
    public sealed class DefaultCharacterControlSessionPolicy : ICharacterControlSessionPolicy
    {
        public CharacterControlSessionPolicyDecision Evaluate(in CharacterControlSessionPolicyContext context)
        {
            // Combat inputs have priority over active control interaction.
            if (context.IsAimPressed)
            {
                return CharacterControlSessionPolicyDecision.Blocked(CharacterControlSessionBlockReason.AimInputActive);
            }

            if (context.CastRequestedThisFrame)
            {
                return CharacterControlSessionPolicyDecision.Blocked(CharacterControlSessionBlockReason.CastRequested);
            }

            if (context.HasActiveControlSession && context.HasMovementInput)
            {
                // Core LT-CORE-002B rule: movement can continue while control session is active.
                return CharacterControlSessionPolicyDecision.Allowed;
            }

            return CharacterControlSessionPolicyDecision.Allowed;
        }
    }
}
