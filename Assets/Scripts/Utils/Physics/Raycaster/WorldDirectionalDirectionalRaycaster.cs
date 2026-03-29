using System;
using UnityEngine;

namespace Utils.Physics.Raycaster
{
    public class WorldDirectionalDirectionalRaycaster : DirectionalRaycaster
    {
        public override event Action<RaycastHit[]> OnRayCollide;

        [SerializeField] private Vector3 _direction;

        public void Update()
        {
            _currentFrame++;
            
            if (_currentFrame >= _frameOffset)
            {
                var rayCastInfo = UnityEngine.Physics.RaycastAll(transform.position, _direction, _raycastDistance);

                if (rayCastInfo is not null && rayCastInfo.Length > 0)
                {
                    OnRayCollide?.Invoke(rayCastInfo);
                }

                _currentFrame = 0;
            }
        }
    }
}