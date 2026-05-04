using System;
using UnityEngine;

namespace Gameplay.Interaction.World
{
    [Serializable]
    public struct MovementAxisIgnoreMask
    {
        public bool x;
        public bool y;
        public bool z;

        public Vector3 Apply(Vector3 direction, float epsilon)
        {
            if (x)
            {
                direction.x = 0f;
            }

            if (y)
            {
                direction.y = 0f;
            }

            if (z)
            {
                direction.z = 0f;
            }

            return direction.sqrMagnitude <= epsilon
                ? Vector3.zero
                : direction.normalized;
        }
    }
}
