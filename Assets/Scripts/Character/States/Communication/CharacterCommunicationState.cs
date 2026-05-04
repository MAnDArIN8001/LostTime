using Character.Modules.Movement;
using FSM;

namespace Character.States.Communication
{
    public sealed class CharacterCommunicationState : State
    {
        private readonly MovementModule _movementModule;

        public CharacterCommunicationState(StateType stateType, MovementModule movementModule) : base(stateType)
        {
            _movementModule = movementModule;
        }

        public override void Enter()
        {
            _movementModule?.Stop();
        }

        public override void Update()
        {
            _movementModule?.Stop();
        }

        public override void Exit()
        {
        }
    }
}
