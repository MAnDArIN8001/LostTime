using System;

namespace HFSM
{
    public class Transition
    {
        public StateType From { get; private set; }
        public StateType To { get; private set; }
        
        public Func<bool> Condition { get; private set; }
        
        public Transition()
    }
}