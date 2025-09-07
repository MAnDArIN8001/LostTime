using UnityEngine;

namespace Character.Modules.Rotation.Variants
{
    public class CharacterRotationModule : RotationModule
    {
        [SerializeField] private float _sensetivity;
        
        [Header("Roots")]
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _camera;
        
        public override void Rotate(Vector2 input)
        {
            var computedInput = input.normalized * _sensetivity;
            
            _body.Rotate(new Vector3(0, computedInput.x, 0));
            _camera.Rotate(new Vector3(0, 0, computedInput.y));
        }
    }
}