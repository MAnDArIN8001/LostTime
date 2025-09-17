using UnityEngine;

namespace Character.Modules.Rotation.Variants
{
    public class CameraRotationModule : RotationModule
    {
        [SerializeField] private float _smoothTime = 0.05f;
        [SerializeField] private float _sensitivity = 1f;

        [Header("Roots")]
        [SerializeField] private Transform _camera;

        [Header("Limits")]
        [SerializeField] private Vector2 _xLimits = new Vector2(-80f, 80f);
        [SerializeField] private Vector2 _yLimits = new Vector2(-360f, 360f);

        private Vector2 _currentInput;
        private Vector2 _inputVelocity;

        private float _yaw;
        private float _pitch;

        public override void Rotate(Vector3 input)
        {
            _currentInput = Vector2.SmoothDamp(_currentInput, input, ref _inputVelocity, _smoothTime);
            
            _yaw   += _currentInput.x * _sensitivity * Time.deltaTime;
            _pitch -= _currentInput.y * _sensitivity * Time.deltaTime;
            
            _yaw   = Mathf.Clamp(_yaw, _yLimits.x, _yLimits.y);
            _pitch = Mathf.Clamp(_pitch, _xLimits.x, _xLimits.y);
            
            _camera.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
    }
}