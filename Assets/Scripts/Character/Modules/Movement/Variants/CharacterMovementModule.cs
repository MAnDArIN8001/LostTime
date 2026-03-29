using UnityEngine;

namespace Character.Modules.Movement.Variants
{
    public class CharacterMovementModule : MovementModule
    {
        [Header("Movement")] 
        [SerializeField] private float _acceleration = 12f;
        [SerializeField] private float _deceleration = 16f;
        [SerializeField] private float _directionLerpSpeed = 12f;
        
        private float _currentSpeed;
        private float _verticalVelocity;

        [Header("Gravity")] 
        [SerializeField] private float _gravity = -9.81f;

        [Space, SerializeField] private CharacterController _characterController;
        
        private Vector3 _currentVelocity;
        private Vector3 _velocityRef;

        public override float MovementSpeed => _currentSpeed;
        
        public override Vector3 Velocity => _currentVelocity;
        
        public override Transform Root => _characterController.transform;

        public override void Move(float targetSpeed, Vector3 direction)
        {
            var targetMagnitude = targetSpeed * direction.magnitude;
            var speedDelta = targetMagnitude > _currentSpeed ? _acceleration : _deceleration;

            _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetMagnitude, speedDelta * Time.deltaTime);

            Vector3 horizontal = new Vector3(_currentVelocity.x, 0, _currentVelocity.z);

            if (direction.sqrMagnitude > 0.001f)
            {
                Vector3 targetHorizontal = direction.normalized * _currentSpeed;

                horizontal = Vector3.Lerp(horizontal, targetHorizontal, _directionLerpSpeed * Time.deltaTime);
            }
            else
            {
                horizontal = Vector3.MoveTowards(horizontal, Vector3.zero, _deceleration * Time.deltaTime);
            }

            _verticalVelocity = _characterController.isGrounded ? 0 : _verticalVelocity + _gravity * Time.deltaTime;

            _currentVelocity = horizontal;
            _currentVelocity.y = _verticalVelocity;

            _characterController.Move(_currentVelocity * Time.deltaTime);
        }

        public override void Stop()
        {
            _currentSpeed = 0;
            _currentVelocity = Vector3.zero;

            _characterController.Move(_currentVelocity);
        }
    }
}