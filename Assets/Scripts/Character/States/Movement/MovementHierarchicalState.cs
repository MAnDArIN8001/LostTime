using FSM;
using HFSM;
using UnityEngine;

namespace Character.States
{
    public class MovementHierarchicalState : HierarchicalState
    {
        private readonly CharacterController _characterController;
        
        public MovementHierarchicalState(StateType stateType, 
            CharacterController characterController) : base(stateType)
        {
            _characterController = characterController;
        }
    }
}