using Character.Modules.Movement;
using Character.Modules.Animation.Facade;
using Character.Modules.Rotation;
using FSM;
using System;
using UnityEngine;

namespace Character.States.Combat
{
    public class CharacterCastState : State
    {
        private readonly MovementModule _movementModule;
        private readonly RotationModule _bodyRotationModule;
        private readonly Transform _camera;
        private readonly float _castLockDuration;
        private readonly Func<bool> _tryCast;
        private readonly Func<bool> _isCastAnimationFinished;
        private readonly IAnimationFacade _animationFacade;
        private readonly string _castAnimationParamId;

        private float _castLockEndTime;
        private bool _areTransitionsBlocked;

        public bool IsLockFinished => Time.time >= _castLockEndTime;
        public bool CanExit => IsLockFinished && !_areTransitionsBlocked;

        public CharacterCastState(
            StateType stateType,
            MovementModule movementModule,
            RotationModule bodyRotationModule,
            Transform camera,
            float castLockDuration,
            Func<bool> tryCast = null,
            Func<bool> isCastAnimationFinished = null,
            IAnimationFacade animationFacade = null,
            string castAnimationParamId = null) : base(stateType)
        {
            _movementModule = movementModule;
            _bodyRotationModule = bodyRotationModule;
            _camera = camera;
            _castLockDuration = Mathf.Max(0f, castLockDuration);
            _tryCast = tryCast;
            _isCastAnimationFinished = isCastAnimationFinished;
            _animationFacade = animationFacade;
            _castAnimationParamId = castAnimationParamId;
        }

        public override void Enter()
        {
            _areTransitionsBlocked = _isCastAnimationFinished != null;
            _movementModule.Stop();
            SnapFacingToCameraForward();

            var castSucceeded = _tryCast?.Invoke() ?? true;
            _castLockEndTime = Time.time + _castLockDuration;
            if (!castSucceeded)
            {
                _castLockEndTime = Time.time;
                _areTransitionsBlocked = false;
            }
            else if (!string.IsNullOrWhiteSpace(_castAnimationParamId))
            {
                _animationFacade?.Set(_castAnimationParamId, true);
            }
        }

        public override void Update()
        {
            if (_areTransitionsBlocked && (_isCastAnimationFinished?.Invoke() ?? false))
            {
                _areTransitionsBlocked = false;
            }

            _movementModule.Stop();
            ApplyFacingTowardCameraForward();
        }

        public override void Exit()
        {
            _areTransitionsBlocked = false;
            _animationFacade?.Set(_castAnimationParamId, false);
        }

        private static Vector3 CameraForwardOnPlane(Transform camera)
        {
            if (camera == null)
            {
                return Vector3.zero;
            }

            return Vector3.ProjectOnPlane(camera.forward, Vector3.up).normalized;
        }

        private void SnapFacingToCameraForward()
        {
            var forward = CameraForwardOnPlane(_camera);
            if (forward.sqrMagnitude < 0.0001f)
            {
                return;
            }

            _movementModule.Root.rotation = Quaternion.LookRotation(forward);
        }

        private void ApplyFacingTowardCameraForward()
        {
            var forward = CameraForwardOnPlane(_camera);
            if (forward.sqrMagnitude < 0.0001f)
            {
                return;
            }

            _bodyRotationModule?.Rotate(forward);
        }
    }
}
