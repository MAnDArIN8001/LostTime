using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public class StateMachine
    {
        private Dictionary<StateType, State> _states;

        private List<StateTransition> _stateTransitions;

        private State _currentState;

        public StateMachine(Dictionary<StateType, State> states, 
            List<StateTransition> stateTransitions,
            StateType defaultState)
        {
            _states = states;
            _stateTransitions = stateTransitions;
            
            ChangeState(defaultState);
        }

        public void Update()
        {
            for (var i = 0; i < _stateTransitions.Count; i++)
            {
                var stateTransition = _stateTransitions[i];
                
                if (stateTransition.From == _currentState?.StateType 
                    && stateTransition.Condition())
                {
                    ChangeState(stateTransition.To);

                    break;
                }
            }
            
            _currentState?.Update();
        }

        public void ChangeState(StateType stateType)
        {
            if (!_states.TryGetValue(stateType, out var state))
            {
                Debug.LogWarning($"The state machine {this} doesn't contains state of type {stateType.ToString()}");

                return;
            }
            
            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }
    }
}