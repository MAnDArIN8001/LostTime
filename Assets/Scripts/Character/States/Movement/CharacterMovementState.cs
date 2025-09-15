using Character.Modules.Animation;
using FSM;
using HFSM;
using UnityEngine;
using Character.Modules.Movement;

namespace Character.States
{
    public class CharacterMovementState : HierarchicalState
    {
        protected readonly float _movementSpeed;
        
        protected readonly MovementModule _movementModule;
        protected readonly AnimationModule _animationModule;

        protected readonly MainInput _mainInput;

        private float _movementAnimationMagnitude;

        protected Vector2 _lastInput;
        
        public CharacterMovementState(StateType stateType, float movementSpeed, MainInput mainInput, MovementModule movementModule, AnimationModule animationModule) : base(stateType)
        {
            _mainInput = mainInput;
            _movementSpeed = movementSpeed;
            _movementModule = movementModule;
            _animationModule = animationModule;
        }

        public override void Update()
        {
            var input = ReadInputValue();
            var direction = ComputeMovementFromInput(input.normalized);

            _movementAnimationMagnitude = Mathf.MoveTowards(_movementAnimationMagnitude, input.normalized.magnitude, 0.05f);
            
            _movementModule.Move(_movementSpeed, direction.normalized);
            _animationModule.SetMovement(_movementAnimationMagnitude, 1);
            
            _lastInput = input;
        }

        protected Vector3 ComputeMovementFromInput(Vector2 input) => _movementModule.Root.forward*input.y + _movementModule.Root.right*input.x;
        
        protected Vector2 ReadInputValue() => _mainInput.Character.Movement.ReadValue<Vector2>();
    }
}