using UnityEngine;

namespace Character.Modules.Rotation.Variants
{
    public class CharacterRotationModule : RotationModule
    {
        [SerializeField] private float _smoothTime = 0.05f;
        [SerializeField] private float _sensetivity;
        
        [Header("Roots")]
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _camera;

        private Vector2 _currentInput;
        private Vector2 _inputVelocity;

        public override void Rotate(Vector2 input)
        {
            _currentInput = Vector2.SmoothDamp(_currentInput, input, ref _inputVelocity, _smoothTime);
            
            var computedInput = _currentInput * _sensetivity * Time.deltaTime;

            _body.Rotate(Vector3.up * computedInput.x);
            _camera.Rotate(Vector3.right * -computedInput.y);
        }
    }
}