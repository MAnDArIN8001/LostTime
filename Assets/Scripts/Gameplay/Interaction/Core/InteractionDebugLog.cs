using UnityEngine;

namespace Gameplay.Interaction.Core
{
    public static class InteractionDebugLog
    {
        public static bool Enabled { get; private set; }
        public static bool VerboseDiscoveryEnabled { get; private set; }

        public static void Configure(bool enabled, bool verboseDiscovery)
        {
            Enabled = enabled;
            VerboseDiscoveryEnabled = enabled && verboseDiscovery;
        }

        public static void Log(Object context, string message)
        {
            if (!Enabled)
            {
                return;
            }

            Debug.Log($"[Interaction] {message}", context);
        }

        public static void LogVerbose(Object context, string message)
        {
            if (!VerboseDiscoveryEnabled)
            {
                return;
            }

            Debug.Log($"[Interaction][Verbose] {message}", context);
        }
    }
}
