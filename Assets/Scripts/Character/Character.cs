using FSM;
using Zenject;
using UnityEngine;
using Character.States;
using System.Collections.Generic;
using Character.Modules.Movement;
using Character.Modules.Rotation;
using Character.Setup;
using StateMachine = FSM.StateMachine;

namespace Character
{
    public class Character : MonoBehaviour
    {
        //TODO move to setup
        [SerializeField] private CharacterSetup _characterSetup;
        
        [Header("Modules")]
        [Space, SerializeField] private MovementModule _movementModule;
        [SerializeField] private RotationModule _rotationModule;
        
        private StateMachine _movementStateMachine;

        [Inject] private MainInput _mainInput;

        private void Awake()
        {
            InitializeMovementStateMachine();
        }

        private void Update()
        {
            var mouseInput = _mainInput.Character.CameraMovement.ReadValue<Vector2>();
            
            _rotationModule.Rotate(mouseInput);
            
            _movementStateMachine.Update();
        }

        private void InitializeMovementStateMachine()
        {
            var movementState = new MovementHierarchicalState(StateType.Movement, StateType.Walk);
            
            movementState.AddChildState(new CharacterMovementState(StateType.Walk, _mainInput, _movementModule, _characterSetup.WalkSpeed));
            movementState.AddStateTransition(new StateTransition(StateType.Run, StateType.Walk, () => _mainInput.Character.RunAction.WasPerformedThisFrame()));
            movementState.AddChildState(new CharacterMovementState(StateType.Run, _mainInput, _movementModule, _characterSetup.RunSpeed));
            movementState.AddStateTransition(new StateTransition(StateType.Walk, StateType.Run, () => _mainInput.Character.RunAction.WasPerformedThisFrame()));

            var idleState = new IdleHierarchicalState(StateType.Idle, StateType.Idle);
            
            var globalStates = new Dictionary<StateType, State>()
            {
                { idleState.StateType, idleState },
                { movementState.StateType, movementState },
            };

            var globalTransitions = new List<StateTransition>()
            {
                new StateTransition(StateType.Idle, StateType.Movement, () => ReadInputValues().magnitude > 0.1f),
                new StateTransition(StateType.Movement, StateType.Idle, () => _movementModule.Velocity.magnitude <= 0.1f && _movementModule.MovementSpeed <= 0.1f)
            };

            _movementStateMachine = new StateMachine(globalStates, globalTransitions, StateType.Idle);
        }

        private Vector2 ReadInputValues() => _mainInput.Character.Movement.ReadValue<Vector2>();
    }
}