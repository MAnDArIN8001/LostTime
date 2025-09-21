using UnityEngine;

namespace Character.Modules.Rotation.Variants
{
    public class BodyRotationModule : RotationModule
    {
        [SerializeField] private float _rotationSpeed;

        [Space, SerializeField] private Transform _root;
        
        public override void Rotate(Vector3 input)
        {
            if (input == Vector3.zero)
            {
                return;
            }
            
            var rotation = Quaternion.LookRotation(input);
            var smoothRotation = Quaternion.Lerp(_root.localRotation, rotation, _rotationSpeed);

            _root.rotation = smoothRotation;
        }
    }
}