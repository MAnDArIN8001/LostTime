using UnityEngine;

namespace Utils.Rotators
{
    [DisallowMultipleComponent]
    public sealed class TargetRotationCopier : MonoBehaviour
    {
        private enum UpdateMode
        {
            Update = 0,
            LateUpdate = 1,
            FixedUpdate = 2,
        }

        [SerializeField] private Transform _target;
        [SerializeField] private Space _space = Space.World;
        [SerializeField] private UpdateMode _updateMode = UpdateMode.LateUpdate;
        [SerializeField] private Vector3 _rotationOffsetEuler;

        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        private void Update()
        {
            if (_updateMode == UpdateMode.Update)
            {
                CopyRotation();
            }
        }

        private void LateUpdate()
        {
            if (_updateMode == UpdateMode.LateUpdate)
            {
                CopyRotation();
            }
        }

        private void FixedUpdate()
        {
            if (_updateMode == UpdateMode.FixedUpdate)
            {
                CopyRotation();
            }
        }

        private void CopyRotation()
        {
            if (_target == null)
            {
                return;
            }

            var rotationOffset = Quaternion.Euler(_rotationOffsetEuler);

            if (_space == Space.Self)
            {
                transform.localRotation = _target.localRotation * rotationOffset;
                return;
            }

            transform.rotation = _target.rotation * rotationOffset;
        }
    }
}
