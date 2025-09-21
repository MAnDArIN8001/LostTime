using FSM;
using HFSM;
using UnityEngine;

namespace Character.States
{
    public class MovementHierarchicalState : HierarchicalState
    {
        private readonly StateType _defaultState;
        
        public MovementHierarchicalState(StateType stateType, StateType defaultState) : base(stateType)
        {
            _defaultState = defaultState;
        }

        public override void Enter()
        {
            base.Enter();
            
            ChangeState(_defaultState);
        }
    }
}