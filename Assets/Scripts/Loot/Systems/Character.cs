using FSM;
using Zenject;
using UnityEngine;
using Character.States;
using System.Collections.Generic;
using Character.Modules.Animation;
using Character.Modules.Animation.Facade;
using Character.Modules.Movement;
using Character.Modules.Rotation;
using Character.Setup;
using Character.States.Combat;
using Combat;
using Loot.Inventory;
using Loot.Systems;
using Utils.Events;
using UnityEngine.InputSystem;

using State = FSM.State;
using StateMachine = FSM.StateMachine;
using StateTransition = FSM.StateTransition;

namespace Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterSetup _characterSetup;
        
        [Header("Modules")]
        [SerializeField] private MovementModule _movementModule;
        [SerializeField] private AnimationModule _animationModule;
        [SerializeField] private RotationModule _rotationModule;
        [SerializeField] private RotationModule _bodyRotationModule;

        [Header("Roots")] 
        [SerializeField] private Transform _camera;

        [Header("Combat")]
        [SerializeField, Min(0f)] private float _castLockDuration = 0.3f;
        [SerializeField] private CharacterSpellCaster _spellCaster;

        [Header("Interaction")]
        [SerializeField] private InteractionController _interactionController;

        private EventBus _animationEventBus;
        private IAnimationFacade _animationFacade;
        
        private StateMachine _movementStateMachine;

        private InventoryService _inventoryService;

        [Inject] private MainInput _mainInput;
        private InputAction _aimAction;

        private void Awake()
        {
            _animationEventBus = new EventBus();
            _animationFacade = new AnimationFacade(_animationEventBus);
            _aimAction = _mainInput.FindAction("Character/Aim");
            
            _animationModule.Initialize(_animationEventBus);

            InitializeMovementStateMachine();
        }

        private void Update()
        {
            var mouseInput = _mainInput.Character.CameraMovement.ReadValue<Vector2>();
            
            _rotationModule.Rotate(mouseInput);
            
            _movementStateMachine.Update();

            if (_mainInput.Character.CommunicationAction.WasPerformedThisFrame())
            {
                _interactionController?.TryInteract(gameObject);
            }
        }

        private void InitializeMovementStateMachine()
        {
            var movementState = new MovementHierarchicalState(StateType.Movement, StateType.Walk, _movementModule);
            var castState = new CharacterCastState(StateType.Attack, _movementModule, _castLockDuration, TryCastSpell);
            
            movementState.AddChildState(new CharacterMovementState(StateType.Walk, _characterSetup.WalkSpeed, _mainInput, _movementModule, _animationFacade, _bodyRotationModule, _camera));
            movementState.AddStateTransition(new StateTransition(StateType.Run, StateType.Walk, () => _mainInput.Character.RunAction.WasPerformedThisFrame()));
            movementState.AddChildState(new CharacterRunState(StateType.Run, _characterSetup.RunSpeed, _mainInput, _movementModule, _animationFacade, _bodyRotationModule, _camera));
            movementState.AddStateTransition(new StateTransition(StateType.Walk, StateType.Run, () => _mainInput.Character.RunAction.WasPerformedThisFrame()));

            var idleState = new IdleHierarchicalState(StateType.Idle, StateType.Idle);
            
            var globalStates = new Dictionary<StateType, State>()
            {
                { idleState.StateType, idleState },
                { movementState.StateType, movementState },
                { castState.StateType, castState },
            };

            var globalTransitions = new List<StateTransition>()
            {
                new StateTransition(StateType.Idle, StateType.Movement, () => ReadInputValues().magnitude > 0.1f),
                new StateTransition(StateType.Movement, StateType.Idle, () => _movementModule.Velocity.magnitude <= 0.1f && _movementModule.MovementSpeed <= 0.1f),
                new StateTransition(StateType.Idle, StateType.Attack, CanEnterCastState),
                new StateTransition(StateType.Movement, StateType.Attack, CanEnterCastState),
                new StateTransition(StateType.Attack, StateType.Movement, () => castState.IsLockFinished && ReadInputValues().magnitude > 0.1f),
                new StateTransition(StateType.Attack, StateType.Idle, () => castState.IsLockFinished && ReadInputValues().magnitude <= 0.1f),
            };

            _movementStateMachine = new StateMachine(globalStates, globalTransitions, StateType.Idle);
        }

        private Vector2 ReadInputValues() => _mainInput.Character.Movement.ReadValue<Vector2>();

        private bool CanEnterCastState() =>
            _mainInput.Character.Cast.WasPerformedThisFrame() &&
            IsAimPressed() &&
            _spellCaster != null &&
            _spellCaster.CanCast;

        private bool IsAimPressed() => _aimAction != null && _aimAction.IsPressed();

        private bool TryCastSpell() => _spellCaster != null && _spellCaster.TryCast();
    }
}