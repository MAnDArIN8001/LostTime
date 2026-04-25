using UnityEngine;

namespace Utils.Rotators
{
    [DisallowMultipleComponent]
    public sealed class FaceCameraRotation : MonoBehaviour
    {
        private enum RotationMode
        {
            Full = 0,
            YAxisOnly = 1,
        }

        [SerializeField] private bool _invertDirection;

        [Space, SerializeField] private Vector3 _rotationOffsetEuler;

        [Space, SerializeField] private Transform _cameraTransform;

        [Space, SerializeField] private RotationMode _rotationMode = RotationMode.Full;

        private void LateUpdate()
        {
            RotateToCamera();
        }

        private void RotateToCamera()
        {
            var cameraTransform = ResolveCameraTransform();
            if (cameraTransform == null)
            {
                return;
            }

            var direction = _invertDirection
                ? transform.position - cameraTransform.position
                : cameraTransform.position - transform.position;

            if (_rotationMode == RotationMode.YAxisOnly)
            {
                direction.y = 0f;
            }

            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            targetRotation *= Quaternion.Euler(_rotationOffsetEuler);
            transform.rotation = targetRotation;
        }

        private Transform ResolveCameraTransform()
        {
            if (_cameraTransform != null)
            {
                return _cameraTransform;
            }

            var mainCamera = UnityEngine.Camera.main;
            return mainCamera != null ? mainCamera.transform : null;
        }
    }
}
