using UnityEngine;

namespace Utils.Followers
{
    [DisallowMultipleComponent]
    public sealed class BackRaycastTargetFollower : MonoBehaviour
    {
        private enum UpdateMode
        {
            Update = 0,
            LateUpdate = 1,
            FixedUpdate = 2,
        }

        [Header("References")]
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Transform _defaultTransform;
        [SerializeField] private Transform _ignoreHierarchyRoot;

        [Header("Movement")]
        [SerializeField] private UpdateMode _updateMode = UpdateMode.LateUpdate;
        [SerializeField, Min(0f)] private float _positionSmoothTime = 0.15f;
        [SerializeField] private Vector3 _defaultOffset;
        [SerializeField, Min(0f)] private float _collisionOffset = 0.15f;

        [Header("Collision")]
        [SerializeField] private LayerMask _layerMask = UnityEngine.Physics.DefaultRaycastLayers;
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.Ignore;

        private readonly RaycastHit[] _raycastHits = new RaycastHit[8];
        private Vector3 _positionVelocity;

        private void Reset()
        {
            _movable = transform;
            _ignoreHierarchyRoot = transform;
        }

        private void Awake()
        {
            if (_movable == null)
            {
                _movable = transform;
            }

            if (_ignoreHierarchyRoot == null)
            {
                _ignoreHierarchyRoot = transform;
            }
        }

        private void Update()
        {
            if (_updateMode == UpdateMode.Update)
            {
                FollowCurrentTarget(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (_updateMode == UpdateMode.LateUpdate)
            {
                FollowCurrentTarget(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (_updateMode == UpdateMode.FixedUpdate)
            {
                FollowCurrentTarget(Time.fixedDeltaTime);
            }
        }

        private void FollowCurrentTarget(float deltaTime)
        {
            if (_movable == null || _targetTransform == null || _defaultTransform == null)
            {
                return;
            }

            var destination = ResolveDestination();
            var smoothTime = Mathf.Max(0.0001f, _positionSmoothTime);

            _movable.position = Vector3.SmoothDamp(
                _movable.position,
                destination,
                ref _positionVelocity,
                smoothTime,
                Mathf.Infinity,
                deltaTime);
        }

        private bool TryGetFirstHitOnPath(Vector3 origin, Vector3 direction, float distance, out RaycastHit firstHit)
        {
            firstHit = default;

            var hitCount = UnityEngine.Physics.RaycastNonAlloc(
                origin,
                direction,
                _raycastHits,
                distance,
                _layerMask,
                _queryTriggerInteraction);

            var hasHit = false;
            var nearestDistance = float.MaxValue;
            for (var index = 0; index < hitCount; index++)
            {
                var hit = _raycastHits[index];
                var hitTransform = hit.transform;
                if (hitTransform == null)
                {
                    continue;
                }

                if (_ignoreHierarchyRoot != null && hitTransform.IsChildOf(_ignoreHierarchyRoot))
                {
                    continue;
                }

                if (hit.distance < nearestDistance)
                {
                    nearestDistance = hit.distance;
                    firstHit = hit;
                    hasHit = true;
                }
            }

            return hasHit;
        }

        private Vector3 GetDesiredPosition()
        {
            return _defaultTransform.position + _defaultOffset;
        }

        private Vector3 ResolveDestination()
        {
            var origin = _targetTransform.position;
            var desiredPosition = GetDesiredPosition();
            var path = desiredPosition - origin;
            var distance = path.magnitude;

            if (distance <= Mathf.Epsilon)
            {
                return desiredPosition;
            }

            var direction = path / distance;
            if (!TryGetFirstHitOnPath(origin, direction, distance, out var hit))
            {
                return desiredPosition;
            }

            var collisionSafePosition = hit.point - direction * _collisionOffset;
            return collisionSafePosition;
        }
    }
}
