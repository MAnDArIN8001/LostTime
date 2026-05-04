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
using Character.States.Communication;
using Combat;
using Gameplay.Interaction.Character;
using Loot.Data;
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
        [SerializeField] private string _castAnimationStateName = "SpellCastAnimation";
        [SerializeField, Min(0)] private int _castAnimationLayer = 0;

        [Header("Interaction")]
        [SerializeField] private InteractionController _interactionController;

        private EventBus _animationEventBus;
        private IAnimationFacade _animationFacade;
        
        private StateMachine _movementStateMachine;
        private CharacterCastState _castState;
        private CharacterInteractionDriver _interactionDriver;

        private InventoryService _inventoryService;

        [Inject] private MainInput _mainInput;
        private InputAction _aimAction;
        private bool _communicationRequested;

        private void Awake()
        {
            _animationEventBus = new EventBus();
            _animationFacade = new AnimationFacade(_animationEventBus);
            _aimAction = _mainInput.FindAction("Character/Aim");
            _interactionDriver = new CharacterInteractionDriver(_interactionController);
            
            _animationModule.Initialize(_animationEventBus);

            InitializeMovementStateMachine();
        }

        private void Update()
        {
            var mouseInput = _mainInput.Character.CameraMovement.ReadValue<Vector2>();
            var movementInput = _mainInput.Character.Movement.ReadValue<Vector2>();
            var castRequested = _mainInput.Character.Cast.WasPerformedThisFrame();
            var isAimPressed = IsAimPressed();
            
            if (!_communicationRequested)
            {
                _rotationModule.Rotate(mouseInput);
            }
            
            _movementStateMachine.Update();

            var interactionFrameInput = new CharacterInteractionFrameInput(
                !_communicationRequested && _mainInput.Character.CommunicationAction.WasPerformedThisFrame(),
                gameObject,
                movementInput,
                isAimPressed,
                castRequested);

            _interactionDriver?.Tick(interactionFrameInput);
        }

        private void InitializeMovementStateMachine()
        {
            var movementState = new MovementHierarchicalState(StateType.Movement, StateType.Walk, _movementModule);
            var aimState = new CharacterAimState(
                StateType.Aim,
                _movementModule,
                _bodyRotationModule,
                _camera);
            _castState = new CharacterCastState(
                StateType.Attack,
                _movementModule,
                _bodyRotationModule,
                _camera,
                _castLockDuration,
                TryCastSpell,
                IsCastAnimationFinished,
                _animationFacade,
                CharacterAnimationKeys.SpellCast);
            
            movementState.AddChildState(new CharacterMovementState(StateType.Walk, _characterSetup.WalkSpeed, _mainInput, _movementModule, _animationFacade, _bodyRotationModule, _camera));
            movementState.AddStateTransition(new StateTransition(StateType.Run, StateType.Walk, () => _mainInput.Character.RunAction.WasPerformedThisFrame()));
            movementState.AddChildState(new CharacterRunState(StateType.Run, _characterSetup.RunSpeed, _mainInput, _movementModule, _animationFacade, _bodyRotationModule, _camera));
            movementState.AddStateTransition(new StateTransition(StateType.Walk, StateType.Run, () => _mainInput.Character.RunAction.WasPerformedThisFrame()));

            var idleState = new IdleHierarchicalState(StateType.Idle, StateType.Idle);
            
            var communicationState = new CharacterCommunicationState(StateType.Communication, _movementModule);

            var globalStates = new Dictionary<StateType, State>()
            {
                { idleState.StateType, idleState },
                { movementState.StateType, movementState },
                { aimState.StateType, aimState },
                { _castState.StateType, _castState },
                { communicationState.StateType, communicationState },
            };

            var globalTransitions = new List<StateTransition>()
            {
                new StateTransition(StateType.Idle, StateType.Aim, IsAimPressed),
                new StateTransition(StateType.Idle, StateType.Movement, () => ReadInputValues().magnitude > 0.1f),
                new StateTransition(StateType.Movement, StateType.Aim, IsAimPressed),
                new StateTransition(StateType.Movement, StateType.Idle, () => _movementModule.Velocity.magnitude <= 0.1f && _movementModule.MovementSpeed <= 0.1f),
                new StateTransition(StateType.Aim, StateType.Attack, CanCastFromAim),
                new StateTransition(StateType.Aim, StateType.Movement, () => !IsAimPressed() && ReadInputValues().magnitude > 0.1f),
                new StateTransition(StateType.Aim, StateType.Idle, () => !IsAimPressed() && ReadInputValues().magnitude <= 0.1f),
                new StateTransition(StateType.Attack, StateType.Aim, () => _castState.CanExit && IsAimPressed()),
                new StateTransition(StateType.Attack, StateType.Movement, () => _castState.CanExit && !IsAimPressed() && ReadInputValues().magnitude > 0.1f),
                new StateTransition(StateType.Attack, StateType.Idle, () => _castState.CanExit && !IsAimPressed() && ReadInputValues().magnitude <= 0.1f),
                new StateTransition(StateType.Idle, StateType.Communication, () => _communicationRequested),
                new StateTransition(StateType.Movement, StateType.Communication, () => _communicationRequested),
                new StateTransition(StateType.Aim, StateType.Communication, () => _communicationRequested),
                new StateTransition(StateType.Attack, StateType.Communication, () => _communicationRequested && _castState.CanExit),
                new StateTransition(StateType.Communication, StateType.Idle, () => !_communicationRequested),
            };

            _movementStateMachine = new StateMachine(globalStates, globalTransitions, StateType.Idle);
        }

        private Vector2 ReadInputValues() => _mainInput.Character.Movement.ReadValue<Vector2>();

        private bool CanCastFromAim() =>
            _mainInput.Character.Cast.WasPerformedThisFrame() &&
            IsAimPressed() &&
            _spellCaster != null &&
            _spellCaster.CanCast;

        private bool IsAimPressed() => _aimAction != null && _aimAction.IsPressed();

        private bool TryCastSpell() => _spellCaster != null && _spellCaster.TryCast();

        private bool IsCastAnimationFinished()
        {
            if (_animationModule == null || _animationModule.Animator == null)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(_castAnimationStateName))
            {
                return true;
            }

            var layerIndex = Mathf.Clamp(_castAnimationLayer, 0, _animationModule.Animator.layerCount - 1);
            var currentState = _animationModule.Animator.GetCurrentAnimatorStateInfo(layerIndex);

            if (currentState.IsName(_castAnimationStateName))
            {
                return currentState.normalizedTime >= 1f;
            }

            if (_animationModule.Animator.IsInTransition(layerIndex))
            {
                var nextState = _animationModule.Animator.GetNextAnimatorStateInfo(layerIndex);
                if (nextState.IsName(_castAnimationStateName))
                {
                    return false;
                }
            }

            // If animator already left cast state, treat the clip as completed.
            return true;
        }

        public void EnterCommunicationState()
        {
            _communicationRequested = true;
            _animationFacade?.Set(CharacterAnimationKeys.Talking, true);
        }

        public void ExitCommunicationState()
        {
            _communicationRequested = false;
            _animationFacade?.Set(CharacterAnimationKeys.Talking, false);
        }
    }
}
