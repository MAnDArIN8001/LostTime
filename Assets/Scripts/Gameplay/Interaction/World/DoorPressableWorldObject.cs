using DG.Tweening;
using Gameplay.Interaction.Authoring;
using Gameplay.Interaction.Core;
using System;
using UnityEngine;

namespace Gameplay.Interaction.World
{
    public sealed class DoorPressableWorldObject : MonoBehaviour, IPressable
    {
        [SerializeField] private bool _toggleOnPress = true;
        [SerializeField] private bool _isLocked;
        [SerializeField] private bool _requirePointerTarget = true;

        [Space, SerializeField] private string _openPrompt = "Open";
        [SerializeField] private string _closePrompt = "Close";
        [SerializeField] private string _lockedPrompt = "Locked";

        [Header("Rotation Configuration")]
        [Space, SerializeField, Min(0f)] private float _openAngle = 90f;
        [SerializeField, Min(0f)] private float _openDuration = 0.35f;
        
        [SerializeField] private Vector3 _rotationAxis = Vector3.up;
        [SerializeField] private Vector3 _doorNormalAxis = Vector3.forward;
        
        [SerializeField] private Transform _rotationPivot;
        
        [SerializeField] private Ease _openEase = Ease.OutCubic;

        private const float Epsilon = 0.0001f;

        private Tween _rotationTween;
        private Quaternion _closedLocalRotation;
        private bool _isOpen;
        private float _currentSignedOpenAngle;

        public string PressPrompt => _isLocked
            ? _lockedPrompt
            : _isOpen && _toggleOnPress
                ? _closePrompt
                : _openPrompt;

        public bool IsLocked
        {
            get => _isLocked;
            set => _isLocked = value;
        }

        public bool IsOpen => _isOpen;

        public event Action<IPressable, GameObject, PointerTargetContext> Pressed;
        public event Action<DoorPressableWorldObject, GameObject> Opened;
        public event Action<DoorPressableWorldObject, GameObject> Closed;
        public event Action<DoorPressableWorldObject, GameObject> LockedPressed;

        private void Awake()
        {
            CacheClosedRotation();
        }

        private void OnEnable()
        {
            CacheClosedRotation();
        }

        private void OnDestroy()
        {
            _rotationTween?.Kill();
            _rotationTween = null;
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

            return ResolvePivot() != null
                && _rotationAxis.sqrMagnitude > Epsilon
                && _doorNormalAxis.sqrMagnitude > Epsilon
                && _openAngle > Epsilon;
        }

        public void Press(GameObject interactor, in PointerTargetContext pointerContext)
        {
            if (_isLocked)
            {
                InteractionDebugLog.Log(this, $"Door '{name}' is locked. Press ignored.");
                LockedPressed?.Invoke(this, interactor);
                return;
            }

            if (!CanPress(interactor, pointerContext))
            {
                InteractionDebugLog.Log(
                    this,
                    $"Door '{name}' rejected press. interactor='{(interactor != null ? interactor.name : "null")}', hasPointerTarget={pointerContext.HasTarget}.");
                return;
            }

            if (_isOpen && _toggleOnPress)
            {
                CloseDoor(interactor, pointerContext);
                return;
            }

            OpenDoor(interactor, pointerContext);
        }

        public void ResetDoorState(bool isOpen = false)
        {
            var pivot = ResolvePivot();
            if (pivot == null)
            {
                return;
            }

            _rotationTween?.Kill();
            _rotationTween = null;

            if (isOpen)
            {
                pivot.localRotation = _closedLocalRotation * Quaternion.AngleAxis(_currentSignedOpenAngle, _rotationAxis.normalized);
            }
            else
            {
                pivot.localRotation = _closedLocalRotation;
            }

            _isOpen = isOpen;
        }

        private void OpenDoor(GameObject interactor, in PointerTargetContext pointerContext)
        {
            var pivot = ResolvePivot();
            if (pivot == null)
            {
                return;
            }

            _currentSignedOpenAngle = ResolveSignedOpenAngle(interactor, pointerContext);
            var targetLocalRotation = _closedLocalRotation * Quaternion.AngleAxis(_currentSignedOpenAngle, _rotationAxis.normalized);

            PlayRotation(pivot, targetLocalRotation, () =>
            {
                _isOpen = true;
                Opened?.Invoke(this, interactor);
            });

            InteractionDebugLog.Log(
                this,
                $"Door '{name}' opened by '{interactor.name}'. signedAngle={_currentSignedOpenAngle:0.###}, pivot='{pivot.name}'.");
            Pressed?.Invoke(this, interactor, pointerContext);
        }

        private void CloseDoor(GameObject interactor, in PointerTargetContext pointerContext)
        {
            var pivot = ResolvePivot();
            if (pivot == null)
            {
                return;
            }

            PlayRotation(pivot, _closedLocalRotation, () =>
            {
                _isOpen = false;
                Closed?.Invoke(this, interactor);
            });

            InteractionDebugLog.Log(this, $"Door '{name}' closed by '{interactor.name}'.");
            Pressed?.Invoke(this, interactor, pointerContext);
        }

        private void PlayRotation(Transform pivot, Quaternion targetLocalRotation, TweenCallback onComplete)
        {
            _rotationTween?.Kill();

            if (_openDuration <= Epsilon)
            {
                pivot.localRotation = targetLocalRotation;
                onComplete?.Invoke();
                _rotationTween = null;
                return;
            }

            _rotationTween = pivot
                .DOLocalRotateQuaternion(targetLocalRotation, _openDuration)
                .SetEase(_openEase)
                .OnComplete(() =>
                {
                    _rotationTween = null;
                    onComplete?.Invoke();
                });
        }

        private float ResolveSignedOpenAngle(GameObject interactor, in PointerTargetContext pointerContext)
        {
            var pivot = ResolvePivot();
            if (pivot == null)
            {
                return _openAngle;
            }

            var worldAxis = pivot.TransformDirection(_rotationAxis.normalized);
            var worldDoorNormal = Vector3.ProjectOnPlane(pivot.TransformDirection(_doorNormalAxis.normalized), worldAxis);
            if (worldDoorNormal.sqrMagnitude <= Epsilon)
            {
                return _openAngle;
            }

            worldDoorNormal.Normalize();

            var playerDirection = ResolvePlayerDirection(pivot, interactor, pointerContext, worldAxis);
            if (playerDirection.sqrMagnitude <= Epsilon)
            {
                return _openAngle;
            }

            playerDirection.Normalize();

            var frontSideDot = Vector3.Dot(playerDirection, worldDoorNormal);
            if (Mathf.Abs(frontSideDot) > Epsilon)
            {
                return frontSideDot > 0f
                    ? -Mathf.Abs(_openAngle)
                    : Mathf.Abs(_openAngle);
            }

            var signedAngleToPlayer = Vector3.SignedAngle(worldDoorNormal, playerDirection, worldAxis);
            return signedAngleToPlayer >= 0f
                ? -Mathf.Abs(_openAngle)
                : Mathf.Abs(_openAngle);
        }

        private static Vector3 ResolvePlayerDirection(
            Transform pivot,
            GameObject interactor,
            in PointerTargetContext pointerContext,
            Vector3 worldAxis)
        {
            if (interactor != null)
            {
                var toInteractor = Vector3.ProjectOnPlane(interactor.transform.position - pivot.position, worldAxis);
                if (toInteractor.sqrMagnitude > Epsilon)
                {
                    return toInteractor;
                }
            }

            if (pointerContext.HasTarget)
            {
                var toHitPoint = Vector3.ProjectOnPlane(pointerContext.HitPoint - pivot.position, worldAxis);
                if (toHitPoint.sqrMagnitude > Epsilon)
                {
                    return toHitPoint;
                }

                var fromHitNormal = Vector3.ProjectOnPlane(-pointerContext.HitNormal, worldAxis);
                if (fromHitNormal.sqrMagnitude > Epsilon)
                {
                    return fromHitNormal;
                }
            }

            return Vector3.zero;
        }

        private void CacheClosedRotation()
        {
            var pivot = ResolvePivot();
            if (pivot == null)
            {
                return;
            }

            _closedLocalRotation = pivot.localRotation;
        }

        private Transform ResolvePivot()
        {
            return _rotationPivot != null ? _rotationPivot : transform;
        }

        private void OnValidate()
        {
            _openPrompt = InteractionAuthoringGuards.NormalizePrompt(_openPrompt, "Open");
            _closePrompt = InteractionAuthoringGuards.NormalizePrompt(_closePrompt, "Close");
            _lockedPrompt = InteractionAuthoringGuards.NormalizePrompt(_lockedPrompt, "Locked");
            _rotationAxis = InteractionAuthoringGuards.NormalizeAxis(_rotationAxis, Vector3.up);
            _doorNormalAxis = InteractionAuthoringGuards.NormalizeAxis(_doorNormalAxis, Vector3.forward);
            _openAngle = InteractionAuthoringGuards.ClampNonNegative(_openAngle);
            _openDuration = InteractionAuthoringGuards.ClampNonNegative(_openDuration);

            if (_requirePointerTarget && !InteractionAuthoringGuards.HasPointerColliderBinding(transform))
            {
                InteractionAuthoringGuards.WarnMissingPointerBinding(this, "Require Pointer Target");
            }
        }
    }
}
