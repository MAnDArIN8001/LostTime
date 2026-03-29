using System;
using UnityEngine;

namespace Utils.Physics.Raycaster
{
    public class LocalDirectionalDirectionalRaycaster : DirectionalRaycaster
    {
        public override event Action<RaycastHit[]> OnRayCollide;
        
        [Space, SerializeField] private Vector3 _directionProfile;
        private Vector3 _direction;

        private void Update() 
        {
            _currentFrame++;
            
            if (_currentFrame >= _frameOffset)
            {
                _direction = 
                    transform.right * _directionProfile.x + transform.forward * _directionProfile.z + transform.up * _directionProfile.y;
                
                var rayCastInfo = UnityEngine.Physics.RaycastAll(transform.position, _direction, _raycastDistance);
                
                if (rayCastInfo is not null && rayCastInfo.Length > 0)
                {
                    OnRayCollide?.Invoke(rayCastInfo);
                }

                _currentFrame = 0;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + _direction * _raycastDistance);
            Gizmos.color = Color.blue;
        }
    }
}