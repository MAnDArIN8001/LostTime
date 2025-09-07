using UnityEngine;

namespace Character.Modules.Movement
{
    public abstract class MovementModule : MonoBehaviour
    {
        public abstract float MovementSpeed { get; }
        
        public abstract Vector3 Velocity { get; }
        
        public abstract Transform Root { get; }
        
        public abstract void Move(float speed, Vector3 direction);
    }
}