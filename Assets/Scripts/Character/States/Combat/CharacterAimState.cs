using Character.Modules.Movement;
using Character.Modules.Rotation;
using FSM;
using UnityEngine;

namespace Character.States.Combat
{
    public class CharacterAimState : State
    {
        private readonly MovementModule _movementModule;
        private readonly RotationModule _bodyRotationModule;
        private readonly Transform _camera;

        public CharacterAimState(
            StateType stateType,
            MovementModule movementModule,
            RotationModule bodyRotationModule,
            Transform camera) : base(stateType)
        {
            _movementModule = movementModule;
            _bodyRotationModule = bodyRotationModule;
            _camera = camera;
        }

        public override void Enter()
        {
            _movementModule.Stop();
        }

        public override void Update()
        {
            _movementModule.Stop();
            ApplyFacingTowardCameraForward();
        }

        public override void Exit()
        {
        }

        private static Vector3 CameraForwardOnPlane(Transform camera)
        {
            if (camera == null)
            {
                return Vector3.zero;
            }

            return Vector3.ProjectOnPlane(camera.forward, Vector3.up).normalized;
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
