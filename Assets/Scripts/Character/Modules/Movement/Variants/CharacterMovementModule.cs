using UnityEngine;

namespace Character.Modules.Movement.Variants
{
    public class CharacterControllerMovementModule : MovementModule
    {
        [Header("Movement")]
        [SerializeField] private float _acceleration = 12f;
        [SerializeField] private float _deceleration = 16f;

        [Header("Gravity")]
        [SerializeField] private float _gravity = -9.81f;

        [Space, SerializeField] private CharacterController _characterController;

        private float _currentSpeed;
        private float _verticalVelocity;

        public float CurrentSpeed => _currentSpeed; 

        public override Transform Root => _characterController.transform;

        public override void Move(float targetSpeed, Vector3 direction)
        {
            float accel = targetSpeed > _currentSpeed ? _acceleration : _deceleration;
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, accel * Time.deltaTime);

            Vector3 horizontal = direction.normalized * _currentSpeed;
            
            if (_characterController.isGrounded)
            {
                if (_verticalVelocity < 0f)
                    _verticalVelocity = -2f;
            }
            else
            {
                _verticalVelocity += _gravity * Time.deltaTime;
            }

            Vector3 velocity = horizontal;
            velocity.y = _verticalVelocity;

            _characterController.Move(velocity * Time.deltaTime);
        }
    }
}