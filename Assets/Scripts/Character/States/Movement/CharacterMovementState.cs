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

        protected readonly MainInput _mainInput;

        protected Vector2 _lastInput;
        
        public CharacterMovementState(StateType stateType, MainInput mainInput, MovementModule movementModule, float movementSpeed) : base(stateType)
        {
            _mainInput = mainInput;
            _movementSpeed = movementSpeed;
            _movementModule = movementModule;
        }

        public override void Update()
        {
            var input = ReadInputValue();
            var direction = ComputeMovementFromInput(input.normalized);
            
            _movementModule.Move(_movementSpeed, direction.normalized);
            
            _lastInput = input;
        }

        protected Vector3 ComputeMovementFromInput(Vector2 input) => _movementModule.Root.forward*input.y + _movementModule.Root.right*input.x;
        
        protected Vector2 ReadInputValue() => _mainInput.Character.Movement.ReadValue<Vector2>();
    }
}