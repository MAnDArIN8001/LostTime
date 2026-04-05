using Character.Modules.Movement;
using Character.Modules.Animation.Facade;
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
        private readonly IAnimationFacade _animationFacade;
        private readonly string _castAnimationParamId;

        private float _castLockEndTime;

        public bool IsLockFinished => Time.time >= _castLockEndTime;

        public CharacterCastState(
            StateType stateType,
            MovementModule movementModule,
            float castLockDuration,
            Func<bool> tryCast = null,
            IAnimationFacade animationFacade = null,
            string castAnimationParamId = null) : base(stateType)
        {
            _movementModule = movementModule;
            _castLockDuration = Mathf.Max(0f, castLockDuration);
            _tryCast = tryCast;
            _animationFacade = animationFacade;
            _castAnimationParamId = castAnimationParamId;
        }

        public override void Enter()
        {
            var castSucceeded = _tryCast?.Invoke() ?? true;
            _castLockEndTime = Time.time + _castLockDuration;
            if (!castSucceeded)
            {
                _castLockEndTime = Time.time;
            }
            else if (!string.IsNullOrWhiteSpace(_castAnimationParamId))
            {
                _animationFacade?.Set(_castAnimationParamId, null);
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
