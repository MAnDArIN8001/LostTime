using UnityEngine;

namespace FSM
{
    public abstract class StateMachineInitializer : MonoBehaviour
    {
        public abstract StateMachine ConstructStateMachine();
    }
}