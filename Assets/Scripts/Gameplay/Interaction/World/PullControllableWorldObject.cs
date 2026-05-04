using DG.Tweening;
using Gameplay.Interaction.Authoring;
using Gameplay.Interaction.Core;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Interaction.World
{
    public class PullControllableWorldObject : MonoBehaviour, IControlable, IPressable
    {
        [SerializeField] private string _controlPrompt = "Pull";
        [SerializeField] private bool _requirePointerTarget = true;
        [SerializeField, Min(0f)] private float _pullSpeed = 2f;
        [SerializeField, Min(0f)] private float _actionStepDistance = 0.75f;
        [SerializeField, Min(0f)] private float _actionMoveDuration = 0.2f;
        [SerializeField] private Ease _actionMoveEase = Ease.OutQuad;
        [SerializeField] private MovementAxisIgnoreMask _ignoredMovementAxes;
        [SerializeField] private LayerMask _movementBlockerLayers = ~0;
        [SerializeField] private bool _blockByTriggers;
        [SerializeField] private bool _snapBackOnControlEnd;
        [SerializeField] private UnityEvent _onControlStarted;
        [SerializeField] private UnityEvent _onControlUpdated;
        [SerializeField] private UnityEvent _onControlEnded;

        private const float Epsilon = 0.0001f;
        private const int CastBufferSize = 16;

        private bool _isControlled;
        private Vector3 _originPosition;
        private ControlMode _activeMode = ControlMode.None;
        private Tween _moveTween;
        private Collider[] _movementColliders = System.Array.Empty<Collider>();
        private readonly RaycastHit[] _castResults = new RaycastHit[CastBufferSize];

        public string ControlPrompt => _controlPrompt;
        public string PressPrompt => _controlPrompt;
        public ControlMode SupportedModes => ControlMode.Pull;
        public bool IsControlled => _isControlled;
        public event Action<GameObject> PressExecuted;
        public event Action<GameObject> ControlStarted;

        private void Awake()
        {
            CacheMovementColliders();
        }

        public bool CanPress(GameObject interactor, in PointerTargetContext pointerContext)
        {
            if (interactor == null)
            {
                return false;
            }

            if (_requirePointerTarget && !pointerContext.HasTarget)
            {
                return false;
            }

            return ResolvePullDirection(interactor).sqrMagnitude > Epsilon && _actionStepDistance > Epsilon;
        }

        public void Press(GameObject interactor, in PointerTargetContext pointerContext)
        {
            if (!CanPress(interactor, pointerContext))
            {
                InteractionDebugLog.Log(this, $"Pull press rejected on '{name}'. interactor='{(interactor != null ? interactor.name : "null")}', hasPointerTarget={pointerContext.HasTarget}, stepDistance={_actionStepDistance:0.###}.");
                return;
            }

            if (!_isControlled)
            {
                _originPosition = transform.position;
            }

            var pullDirection = ResolvePullDirection(interactor);
            var nextPosition = transform.position + pullDirection * _actionStepDistance;
            var moveDelta = nextPosition - transform.position;
            if (moveDelta.sqrMagnitude > Epsilon && IsMovementBlocked(moveDelta, out var blockerName))
            {
                InteractionDebugLog.Log(this, $"Pull press blocked on '{name}' by '{blockerName}'.");
                return;
            }

            _moveTween?.Kill();
            _onControlStarted?.Invoke();

            if (_actionMoveDuration <= Epsilon)
            {
                transform.position = nextPosition;
                _onControlUpdated?.Invoke();
                _onControlEnded?.Invoke();
            }
            else
            {
                _moveTween = transform
                    .DOMove(nextPosition, _actionMoveDuration)
                    .SetEase(_actionMoveEase)
                    .OnUpdate(() => _onControlUpdated?.Invoke())
                    .OnComplete(() =>
                    {
                        _moveTween = null;
                        _onControlEnded?.Invoke();
                    });
            }

            PressExecuted?.Invoke(interactor);
            InteractionDebugLog.Log(this, $"Pull press executed on '{name}'. stepDistance={_actionStepDistance:0.###}, targetPosition={nextPosition}, duration={_actionMoveDuration:0.###}.");
        }

        public bool CanControl(ControlMode mode, GameObject interactor, in PointerTargetContext pointerContext)
        {
            if (mode != ControlMode.Pull || interactor == null)
            {
                return false;
            }

            if (_requirePointerTarget && !pointerContext.HasTarget)
            {
                return false;
            }

            return ResolvePullDirection(interactor).sqrMagnitude > Epsilon;
        }

        public void BeginControl(ControlMode mode, GameObject interactor, in PointerTargetContext pointerContext)
        {
            if (!CanControl(mode, interactor, pointerContext))
            {
                InteractionDebugLog.Log(this, $"Pull control begin rejected on '{name}'. interactor='{(interactor != null ? interactor.name : "null")}', hasPointerTarget={pointerContext.HasTarget}.");
                return;
            }

            if (!_isControlled)
            {
                _originPosition = transform.position;
            }

            _moveTween?.Kill();
            _moveTween = null;
            _isControlled = true;
            _activeMode = mode;
            InteractionDebugLog.Log(this, $"Pull control began on '{name}' by '{interactor.name}'. origin={_originPosition}.");
            _onControlStarted?.Invoke();
            ControlStarted?.Invoke(interactor);
        }

        public void UpdateControl(ControlMode mode, GameObject interactor, in PointerTargetContext pointerContext)
        {
            if (!_isControlled || _activeMode != mode || mode != ControlMode.Pull)
            {
                return;
            }

            var pullDirection = ResolvePullDirection(interactor);
            if (pullDirection.sqrMagnitude <= Epsilon)
            {
                return;
            }

            var nextPosition = transform.position + pullDirection * (_pullSpeed * Time.deltaTime);
            var moveDelta = nextPosition - transform.position;
            if (moveDelta.sqrMagnitude > Epsilon && IsMovementBlocked(moveDelta, out var blockerName))
            {
                InteractionDebugLog.LogVerbose(this, $"Pull control blocked on '{name}' by '{blockerName}'.");
                return;
            }

            transform.position = nextPosition;
            InteractionDebugLog.LogVerbose(this, $"Pull control updated '{name}'. direction={pullDirection}, position={transform.position}.");
            _onControlUpdated?.Invoke();
        }

        public void EndControl(ControlMode mode, GameObject interactor, in PointerTargetContext pointerContext)
        {
            if (!_isControlled || _activeMode != mode)
            {
                return;
            }

            _moveTween?.Kill();
            _moveTween = null;
            _isControlled = false;
            _activeMode = ControlMode.None;

            if (_snapBackOnControlEnd)
            {
                transform.position = _originPosition;
            }

            InteractionDebugLog.Log(this, $"Pull control ended on '{name}'. finalPosition={transform.position}, snapBack={_snapBackOnControlEnd}.");
            _onControlEnded?.Invoke();
        }

        private void OnDestroy()
        {
            _moveTween?.Kill();
            _moveTween = null;
        }

        private Vector3 ResolvePullDirection(GameObject interactor)
        {
            if (interactor == null)
            {
                return Vector3.zero;
            }

            var toInteractor = interactor.transform.position - transform.position;
            if (toInteractor.sqrMagnitude <= Epsilon)
            {
                return Vector3.zero;
            }

            return _ignoredMovementAxes.Apply(toInteractor, Epsilon);
        }

        private void CacheMovementColliders()
        {
            var colliders = GetComponentsInChildren<Collider>(true);
            if (colliders == null || colliders.Length == 0)
            {
                _movementColliders = System.Array.Empty<Collider>();
                return;
            }

            var validCount = 0;
            for (var i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null && colliders[i].enabled)
                {
                    validCount++;
                }
            }

            if (validCount == 0)
            {
                _movementColliders = System.Array.Empty<Collider>();
                return;
            }

            var filtered = new Collider[validCount];
            var index = 0;
            for (var i = 0; i < colliders.Length; i++)
            {
                var collider = colliders[i];
                if (collider == null || !collider.enabled)
                {
                    continue;
                }

                filtered[index++] = collider;
            }

            _movementColliders = filtered;
        }

        private bool IsMovementBlocked(Vector3 moveDelta, out string blockerName)
        {
            blockerName = null;

            var distance = moveDelta.magnitude;
            if (distance <= Epsilon)
            {
                return false;
            }

            var direction = moveDelta / distance;
            var triggerInteraction = _blockByTriggers
                ? QueryTriggerInteraction.Collide
                : QueryTriggerInteraction.Ignore;

            if (_movementColliders.Length == 0)
            {
                if (Physics.Raycast(transform.position, direction, out var rayHit, distance, _movementBlockerLayers, triggerInteraction))
                {
                    if (IsBlockingHit(rayHit.collider))
                    {
                        blockerName = rayHit.collider != null ? rayHit.collider.name : "Unknown";
                        return true;
                    }
                }

                return false;
            }

            for (var i = 0; i < _movementColliders.Length; i++)
            {
                var sourceCollider = _movementColliders[i];
                if (sourceCollider == null || !sourceCollider.enabled)
                {
                    continue;
                }

                var hitCount = CastCollider(sourceCollider, direction, distance, triggerInteraction);
                for (var hitIndex = 0; hitIndex < hitCount; hitIndex++)
                {
                    var hit = _castResults[hitIndex];
                    if (!IsBlockingHit(hit.collider))
                    {
                        continue;
                    }

                    blockerName = hit.collider != null ? hit.collider.name : "Unknown";
                    return true;
                }
            }

            return false;
        }

        private int CastCollider(Collider sourceCollider, Vector3 direction, float distance, QueryTriggerInteraction triggerInteraction)
        {
            switch (sourceCollider)
            {
                case BoxCollider box:
                    return CastBoxCollider(box, direction, distance, triggerInteraction);
                case SphereCollider sphere:
                    return CastSphereCollider(sphere, direction, distance, triggerInteraction);
                case CapsuleCollider capsule:
                    return CastCapsuleCollider(capsule, direction, distance, triggerInteraction);
                default:
                    return CastBoundsFallback(sourceCollider, direction, distance, triggerInteraction);
            }
        }

        private int CastBoxCollider(BoxCollider box, Vector3 direction, float distance, QueryTriggerInteraction triggerInteraction)
        {
            var center = box.transform.TransformPoint(box.center);
            var scale = box.transform.lossyScale;
            var halfExtents = Vector3.Scale(box.size * 0.5f, new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

            return Physics.BoxCastNonAlloc(
                center,
                halfExtents,
                direction,
                _castResults,
                box.transform.rotation,
                distance,
                _movementBlockerLayers,
                triggerInteraction);
        }

        private int CastSphereCollider(SphereCollider sphere, Vector3 direction, float distance, QueryTriggerInteraction triggerInteraction)
        {
            var center = sphere.transform.TransformPoint(sphere.center);
            var scale = sphere.transform.lossyScale;
            var radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            var radius = sphere.radius * radiusScale;

            return Physics.SphereCastNonAlloc(
                center,
                radius,
                direction,
                _castResults,
                distance,
                _movementBlockerLayers,
                triggerInteraction);
        }

        private int CastCapsuleCollider(CapsuleCollider capsule, Vector3 direction, float distance, QueryTriggerInteraction triggerInteraction)
        {
            GetCapsuleWorld(capsule, out var point0, out var point1, out var radius);

            return Physics.CapsuleCastNonAlloc(
                point0,
                point1,
                radius,
                direction,
                _castResults,
                distance,
                _movementBlockerLayers,
                triggerInteraction);
        }

        private int CastBoundsFallback(Collider sourceCollider, Vector3 direction, float distance, QueryTriggerInteraction triggerInteraction)
        {
            var bounds = sourceCollider.bounds;
            if (bounds.extents.sqrMagnitude <= Epsilon)
            {
                return Physics.RaycastNonAlloc(
                    new Ray(bounds.center, direction),
                    _castResults,
                    distance,
                    _movementBlockerLayers,
                    triggerInteraction);
            }

            return Physics.BoxCastNonAlloc(
                bounds.center,
                bounds.extents,
                direction,
                _castResults,
                Quaternion.identity,
                distance,
                _movementBlockerLayers,
                triggerInteraction);
        }

        private static void GetCapsuleWorld(CapsuleCollider capsule, out Vector3 point0, out Vector3 point1, out float radius)
        {
            var t = capsule.transform;
            var center = t.TransformPoint(capsule.center);
            var scale = t.lossyScale;
            var absScale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));

            Vector3 axis;
            float axisScale;
            float radiusScale;

            switch (capsule.direction)
            {
                case 0:
                    axis = t.right;
                    axisScale = absScale.x;
                    radiusScale = Mathf.Max(absScale.y, absScale.z);
                    break;
                case 2:
                    axis = t.forward;
                    axisScale = absScale.z;
                    radiusScale = Mathf.Max(absScale.x, absScale.y);
                    break;
                default:
                    axis = t.up;
                    axisScale = absScale.y;
                    radiusScale = Mathf.Max(absScale.x, absScale.z);
                    break;
            }

            radius = capsule.radius * radiusScale;
            var height = Mathf.Max(capsule.height * axisScale, radius * 2f);
            var halfSegment = Mathf.Max(0f, (height * 0.5f) - radius);
            var offset = axis * halfSegment;

            point0 = center + offset;
            point1 = center - offset;
        }

        private bool IsBlockingHit(Collider hitCollider)
        {
            if (hitCollider == null)
            {
                return false;
            }

            if (hitCollider.transform.IsChildOf(transform))
            {
                return false;
            }

            if (!_blockByTriggers && hitCollider.isTrigger)
            {
                return false;
            }

            var hitLayer = hitCollider.gameObject.layer;
            var hitLayerMask = 1 << hitLayer;
            return (_movementBlockerLayers.value & hitLayerMask) != 0;
        }

        private void OnValidate()
        {
            _controlPrompt = InteractionAuthoringGuards.NormalizePrompt(_controlPrompt, "Pull");

            _pullSpeed = InteractionAuthoringGuards.ClampNonNegative(_pullSpeed);
            _actionStepDistance = InteractionAuthoringGuards.ClampNonNegative(_actionStepDistance);
            _actionMoveDuration = InteractionAuthoringGuards.ClampNonNegative(_actionMoveDuration);

            if (_requirePointerTarget && !InteractionAuthoringGuards.HasPointerColliderBinding(transform))
            {
                InteractionAuthoringGuards.WarnMissingPointerBinding(this, "Require Pointer Target");
            }

            if (_pullSpeed <= Epsilon)
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"{nameof(PullControllableWorldObject)}: pull speed is zero, so active pull control will not move the object.",
                    this);
#endif
            }

            if (_actionStepDistance <= Epsilon)
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"{nameof(PullControllableWorldObject)}: action step distance is zero, so single-press pull will not move the object.",
                    this);
#endif
            }

            CacheMovementColliders();
        }
    }
}
