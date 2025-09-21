using FSM;
using HFSM;

namespace Character.States
{
    public class IdleHierarchicalState : HierarchicalState
    {
        private readonly StateType _defaultState;
        
        public IdleHierarchicalState(StateType stateType, StateType defaultState) : base(stateType)
        {
            _defaultState = defaultState;
        }
    }
}