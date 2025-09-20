using Character.Modules.Animation;
using FSM;
using HFSM;
using UnityEngine;
using Character.Modules.Movement;
using Character.Modules.Rotation;

namespace Character.States
{
    public class CharacterMovementState : HierarchicalState
    {
        private readonly float _movementSpeed;

        private readonly Transform _camera;

        private readonly RotationModule _rotationModule;
        private readonly MovementModule _movementModule;
        private readonly AnimationModule _animationModule;

        private readonly MainInput _mainInput;

        private float _movementAnimationMagnitude;
        
        public CharacterMovementState(StateType stateType, float movementSpeed, MainInput mainInput, MovementModule movementModule, AnimationModule animationModule, RotationModule rotationModule, Transform camera) : base(stateType)
        {
            _mainInput = mainInput;
            _movementSpeed = movementSpeed;
            _rotationModule = rotationModule;
            _movementModule = movementModule;
            _animationModule = animationModule;
            _camera = camera;
        }

        public override void Update()
        {
            var input = ReadInputValue();
            var direction = ComputeMovementFromInput(input.normalized);

            _movementAnimationMagnitude = Mathf.MoveTowards(_movementAnimationMagnitude, input.normalized.magnitude, 0.05f);
            
            _rotationModule.Rotate(direction);
            
            _movementModule.Move(_movementSpeed, direction.normalized);
            _animationModule.SetMovement(_movementAnimationMagnitude, 1);
        }

        private Vector3 ComputeMovementFromInput(Vector2 input)
        {
            var bodyForward = _movementModule.Root.forward;
            var cameraForward = Vector3.ProjectOnPlane(_camera.forward, Vector3.up).normalized;
            
            var forwardDirection = cameraForward * input.y;
            var rightDirection = _camera.right * input.x;

            return (forwardDirection + rightDirection).normalized;
        }
        
        private Vector2 ReadInputValue() => _mainInput.Character.Movement.ReadValue<Vector2>();
    }
}