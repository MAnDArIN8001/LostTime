using UnityEngine;

namespace Character.Modules.Rotation
{
    public abstract class RotationModule : MonoBehaviour
    {
        public abstract void Rotate(Vector3 input);
    }
}