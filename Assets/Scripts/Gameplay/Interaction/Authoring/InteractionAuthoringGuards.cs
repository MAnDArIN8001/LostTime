using UnityEngine;

namespace Gameplay.Interaction.Authoring
{
    internal static class InteractionAuthoringGuards
    {
        public const float AxisEpsilon = 0.0001f;

        public static string NormalizePrompt(string prompt, string fallback)
        {
            return string.IsNullOrWhiteSpace(prompt)
                ? fallback
                : prompt.Trim();
        }

        public static Vector3 NormalizeAxis(Vector3 axis, Vector3 fallback)
        {
            return axis.sqrMagnitude <= AxisEpsilon
                ? fallback
                : axis.normalized;
        }

        public static float ClampNonNegative(float value)
        {
            return Mathf.Max(0f, value);
        }

        public static Space NormalizeSpace(Space movementSpace)
        {
            return movementSpace == Space.World || movementSpace == Space.Self
                ? movementSpace
                : Space.World;
        }

        public static bool HasPointerColliderBinding(Transform root)
        {
            if (root == null)
            {
                return false;
            }

            return root.GetComponentInChildren<Collider>(true) != null;
        }

        public static void WarnMissingPointerBinding(MonoBehaviour owner, string modeName)
        {
#if UNITY_EDITOR
            Debug.LogWarning(
                $"{owner.GetType().Name}: '{modeName}' requires pointer target, but no Collider was found on this object or its children.",
                owner);
#endif
        }
    }
}
