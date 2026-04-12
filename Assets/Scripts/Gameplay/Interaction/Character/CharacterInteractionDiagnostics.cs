namespace Gameplay.Interaction.Character
{
    public static class CharacterInteractionDiagnostics
    {
        private static CharacterInteractionDiagnosticsSnapshot _latest;

        public static int Version { get; private set; }

        public static CharacterInteractionDiagnosticsSnapshot Latest => _latest;

        public static void Publish(in CharacterInteractionDiagnosticsSnapshot snapshot)
        {
            _latest = snapshot;
            Version++;
        }
    }
}
