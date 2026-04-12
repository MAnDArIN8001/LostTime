using DG.Tweening;
using Gameplay.Interaction.Authoring;
using Gameplay.Interaction.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Interaction.World
{
    public class PullControllableWorldObject : MonoBehaviour, IControlable, IPressable
    {
        [SerializeField] private string _controlPrompt = "Pull";
        [SerializeField] private bool _requirePointerTarget = true;
        [SerializeField] private Space _movementSpace = Space.World;
        [SerializeField] private Vector3 _pullAxis = Vector3.forward;
        [SerializeField, Min(0f)] private float _pullSpeed = 2f;
        [SerializeField, Min(0f)] private float _actionStepDistance = 0.75f;
        [SerializeField, Min(0f)] private float _actionMoveDuration = 0.2f;
        [SerializeField] private Ease _actionMoveEase = Ease.OutQuad;
        [SerializeField, Min(0f)] private float _maxPullDistance = 2f;
        [SerializeField] private bool _snapBackOnControlEnd;
        [SerializeField] private UnityEvent _onControlStarted;
        [SerializeField] private UnityEvent _onControlUpdated;
        [SerializeField] private UnityEvent _onControlEnded;

        private const float Epsilon = 0.0001f;

        private bool _isControlled;
        private Vector3 _originPosition;
        private ControlMode _activeMode = ControlMode.None;
        private Tween _moveTween;

        public string ControlPrompt => _controlPrompt;
        public string PressPrompt => _controlPrompt;
        public ControlMode SupportedModes => ControlMode.Pull;
        public bool IsControlled => _isControlled;

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

            return ResolveAxisWorld().sqrMagnitude > Epsilon && _actionStepDistance > Epsilon;
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

            var axis = ResolveAxisWorld();
            var pullDirection = ResolvePullDirection(axis, interactor, pointerContext);
            var nextPosition = transform.position + pullDirection * _actionStepDistance;
            var targetPosition = ClampToRange(nextPosition, axis);

            _moveTween?.Kill();
            _onControlStarted?.Invoke();

            if (_actionMoveDuration <= Epsilon)
            {
                transform.position = targetPosition;
                _onControlUpdated?.Invoke();
                _onControlEnded?.Invoke();
            }
            else
            {
                _moveTween = transform
                    .DOMove(targetPosition, _actionMoveDuration)
                    .SetEase(_actionMoveEase)
                    .OnUpdate(() => _onControlUpdated?.Invoke())
                    .OnComplete(() =>
                    {
                        _moveTween = null;
                        _onControlEnded?.Invoke();
                    });
            }

            InteractionDebugLog.Log(this, $"Pull press executed on '{name}'. stepDistance={_actionStepDistance:0.###}, targetPosition={targetPosition}, duration={_actionMoveDuration:0.###}.");
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

            return ResolveAxisWorld().sqrMagnitude > Epsilon;
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
            InteractionDebugLog.Log(this, $"Pull control began on '{name}' by '{interactor.name}'. origin={_originPosition}, axis={ResolveAxisWorld()}.");
            _onControlStarted?.Invoke();
        }

        public void UpdateControl(ControlMode mode, GameObject interactor, in PointerTargetContext pointerContext)
        {
            if (!_isControlled || _activeMode != mode || mode != ControlMode.Pull)
            {
                return;
            }

            var axis = ResolveAxisWorld();
            if (axis.sqrMagnitude <= Epsilon)
            {
                return;
            }

            var pullDirection = ResolvePullDirection(axis, interactor, pointerContext);
            if (pullDirection.sqrMagnitude <= Epsilon)
            {
                return;
            }

            var nextPosition = transform.position + pullDirection * (_pullSpeed * Time.deltaTime);
            transform.position = ClampToRange(nextPosition, axis);
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

        private Vector3 ResolveAxisWorld()
        {
            if (_pullAxis.sqrMagnitude <= Epsilon)
            {
                return Vector3.zero;
            }

            var normalizedAxis = _pullAxis.normalized;
            return _movementSpace == Space.Self
                ? transform.TransformDirection(normalizedAxis).normalized
                : normalizedAxis;
        }

        private Vector3 ResolvePullDirection(Vector3 axis, GameObject interactor, in PointerTargetContext pointerContext)
        {
            var pointerInfluence = Vector3.Dot(pointerContext.HitNormal, axis);
            if (Mathf.Abs(pointerInfluence) > Epsilon)
            {
                return axis * Mathf.Sign(pointerInfluence);
            }

            if (interactor != null)
            {
                var toInteractor = interactor.transform.position - transform.position;
                var interactorInfluence = Vector3.Dot(toInteractor, axis);
                if (Mathf.Abs(interactorInfluence) > Epsilon)
                {
                    return axis * Mathf.Sign(interactorInfluence);
                }
            }

            var toHitPoint = pointerContext.HitPoint - transform.position;
            var hitPointInfluence = Vector3.Dot(toHitPoint, axis);
            if (Mathf.Abs(hitPointInfluence) > Epsilon)
            {
                return axis * Mathf.Sign(hitPointInfluence);
            }

            return -axis;
        }

        private Vector3 ClampToRange(Vector3 targetPosition, Vector3 axis)
        {
            if (_maxPullDistance <= Epsilon)
            {
                return targetPosition;
            }

            var offsetFromOrigin = targetPosition - _originPosition;
            var offsetOnAxis = Vector3.Dot(offsetFromOrigin, axis);
            var clampedOffset = Mathf.Clamp(offsetOnAxis, -_maxPullDistance, _maxPullDistance);

            return _originPosition + axis * clampedOffset;
        }

        private void OnValidate()
        {
            _controlPrompt = InteractionAuthoringGuards.NormalizePrompt(_controlPrompt, "Pull");
            _movementSpace = InteractionAuthoringGuards.NormalizeSpace(_movementSpace);
            _pullAxis = InteractionAuthoringGuards.NormalizeAxis(_pullAxis, Vector3.forward);

            _pullSpeed = InteractionAuthoringGuards.ClampNonNegative(_pullSpeed);
            _actionStepDistance = InteractionAuthoringGuards.ClampNonNegative(_actionStepDistance);
            _actionMoveDuration = InteractionAuthoringGuards.ClampNonNegative(_actionMoveDuration);
            _maxPullDistance = InteractionAuthoringGuards.ClampNonNegative(_maxPullDistance);

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
        }
    }
}
