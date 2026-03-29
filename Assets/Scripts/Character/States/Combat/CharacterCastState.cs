using Character.Modules.Movement;
using FSM;
using System;
using UnityEngine;

namespace Character.States.Combat
{
    public class CharacterCastState : State
    {
        private readonly MovementModule _movementModule;
        private readonly float _castLockDuration;
        private readonly Func<bool> _tryCast;

        private float _castLockEndTime;

        public bool IsLockFinished => Time.time >= _castLockEndTime;

        public CharacterCastState(StateType stateType, MovementModule movementModule, float castLockDuration, Func<bool> tryCast = null) : base(stateType)
        {
            _movementModule = movementModule;
            _castLockDuration = Mathf.Max(0f, castLockDuration);
            _tryCast = tryCast;
        }

        public override void Enter()
        {
            var castSucceeded = _tryCast?.Invoke() ?? true;
            _castLockEndTime = Time.time + _castLockDuration;
            if (!castSucceeded)
            {
                _castLockEndTime = Time.time;
            }

            _movementModule.Stop();
        }

        public override void Update()
        {
            _movementModule.Stop();
        }

        public override void Exit()
        {
        }
    }
}
