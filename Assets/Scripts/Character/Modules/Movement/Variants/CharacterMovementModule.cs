using UnityEngine;

namespace Character.Modules.Movement.Variants
{
    public class CharacterMovementModule : MovementModule
    {
        [Header("Movement")]
        [SerializeField] private float _acceleration = 12f;
        [SerializeField] private float _deceleration = 16f;

        [Header("Gravity")]
        [SerializeField] private float _gravity = -9.81f;

        [Space, SerializeField] private CharacterController _characterController;

        private float _currentSpeed;
        private float _verticalVelocity;

        private Vector3 _currentVelocity = Vector3.zero;
        
        public override float MovementSpeed => _currentSpeed;

        public override Vector3 Velocity => _currentVelocity;

        public override Transform Root => _characterController.transform;

        public override void Move(float targetSpeed, Vector3 direction)
        {
            var speedDelta = targetSpeed * direction.magnitude > _currentSpeed ? _acceleration : _deceleration;
            
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed * direction.magnitude, speedDelta * Time.deltaTime);
            _currentVelocity = (_currentVelocity + direction).normalized * _currentSpeed;
            
            _verticalVelocity = _characterController.isGrounded ? 0 :  _verticalVelocity + _gravity * Time.deltaTime;
            
            _currentVelocity.y = _verticalVelocity;

            _characterController.Move(_currentVelocity * Time.deltaTime);
        }
    }
}