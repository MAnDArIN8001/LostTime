using Character.Modules.Movement;
using FSM;
using HFSM;
using UnityEngine;

namespace Character.States
{
    public class MovementHierarchicalState : HierarchicalState
    {
        private readonly StateType _defaultState;

        private readonly MovementModule _movementModule;
        
        public MovementHierarchicalState(StateType stateType, StateType defaultState, MovementModule movementModule) : base(stateType)
        {
            _defaultState = defaultState;
            _movementModule = movementModule;
        }

        public override void Enter()
        {
            base.Enter();
            
            ChangeState(_defaultState);
        }

        public override void Exit()
        {
            base.Exit();
            
            _movementModule.Stop();
        }
    }
}