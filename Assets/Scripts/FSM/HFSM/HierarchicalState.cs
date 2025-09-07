using System.Collections.Generic;
using FSM;
using UnityEngine;

namespace HFSM
{
    public abstract class HierarchicalStates : State
    {
        protected HierarchicalStates _activeChild;

        protected Dictionary<StateType, HierarchicalStates> _childStates = new ();

        protected List<StateTransition> _stateTransitions = new ();

        public override void Enter()
        {
            _activeChild?.Enter();
        }

        public override void Update()
        {
            CheckTransitions();
            
            _activeChild?.Update();
        }

        public override void Exit()
        {
            _activeChild?.Exit();
        }
        
        public virtual void AddChildState(HierarchicalStates child)
        {
            _childStates.Add(child.StateType, child);
        }

        public virtual void AddStateTransition(StateTransition stateTransition)
        {
            _stateTransitions.Add(stateTransition);
        }

        protected void CheckTransitions()
        {
            for (var i = 0; i < _stateTransitions.Count; i++)
            {
                var stateTransition = _stateTransitions[i];
                
                if (stateTransition.From == _activeChild?.StateType 
                    && stateTransition.Condition())
                {
                    ChangeState(stateTransition.To);

                    return;
                }
            }
        }
        
        protected void ChangeState(StateType stateType)
        {
            if (!_childStates.TryGetValue(stateType, out var state))
            {
                Debug.LogWarning($"The state machine {this} doesn't contains state of type {stateType.ToString()}");

                return;
            }
            
            _activeChild?.Exit();
            _activeChild = state;
            _activeChild.Enter();
        }
    }
}