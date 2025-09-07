using System;

namespace FSM
{
    public class StateTransition
    {
        public StateType From { get; private set; }
        public StateType To { get; private set; }
        
        public Func<bool> Condition { get; private set; }

        public StateTransition(StateType from, StateType to, Func<bool> condition)
        {
            From = from;
            To = to;
            Condition = condition;
        }
    }
}