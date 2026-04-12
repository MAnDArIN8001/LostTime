namespace Gameplay.Interaction.Character
{
    public readonly struct CharacterControlSessionPolicyDecision
    {
        public CharacterControlSessionPolicyDecision(
            bool allowControlSession,
            bool allowParallelMovement,
            CharacterControlSessionBlockReason blockReason)
        {
            AllowControlSession = allowControlSession;
            AllowParallelMovement = allowParallelMovement;
            BlockReason = blockReason;
        }

        public bool AllowControlSession { get; }
        public bool AllowParallelMovement { get; }
        public CharacterControlSessionBlockReason BlockReason { get; }

        public static CharacterControlSessionPolicyDecision Allowed =>
            new(true, true, CharacterControlSessionBlockReason.None);

        public static CharacterControlSessionPolicyDecision Blocked(CharacterControlSessionBlockReason reason)
        {
            return new CharacterControlSessionPolicyDecision(false, false, reason);
        }
    }
}
