using UnityEngine;

namespace Camera
{
    public class CameraPositionComposer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _positionSmoothTime = 0.2f;

        [Space, SerializeField] private Vector3 _offset = new Vector3(0, 5, -10);
        
		[Header("Target")]
        [SerializeField] private Transform _target;

        private Vector3 _velocity;

        private void LateUpdate()
        {
            if (_target is null)
			{
                return;
			}
            
            Vector3 targetPosition = _target.position + _offset;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref _velocity,
                _positionSmoothTime
            );
        }
    }
}