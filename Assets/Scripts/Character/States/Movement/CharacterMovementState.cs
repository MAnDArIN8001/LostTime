using Character.Modules.Movement;
using Character.Setup;
using FSM;
using HFSM;

namespace Character.States
{
    public class MovementState : HierarchicalState
    {
        protected readonly float _movementSpeed;
        
        protected readonly MovementModule _movementModule;
        
        protected readonly MainInput 
        
        public MovementState(StateType stateType, MainInput mainInput, MovementModule movementModule, float movementSpeed) : base(stateType)
        {
            _movementSpeed = movementSpeed;
            _movementModule = movementModule;
        }
    }
}