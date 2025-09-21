namespace FSM
{
    public abstract class State
    {
        public StateType StateType { get; protected set; }

        public State(StateType stateType)
        {
            StateType = stateType;
        }
        
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}