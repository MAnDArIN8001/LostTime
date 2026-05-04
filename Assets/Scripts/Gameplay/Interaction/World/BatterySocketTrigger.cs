using System;
using DG.Tweening;
using Gameplay.Interaction.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Interaction.World
{
    [DisallowMultipleComponent]
    public sealed class BatterySocketTrigger : MonoBehaviour
    {
        [Header("Socket")]
        [SerializeField] private Transform _socketRoot;
        [SerializeField] private bool _consumeOnce = true;

        [Header("Attach Tween")]
        [SerializeField, Min(0f)] private float _attachDuration = 0.3f;
        [SerializeField] private Ease _attachEase = Ease.OutCubic;
        [SerializeField] private bool _matchSocketRotation = true;

        [Header("Push/Pull Disable")]
        [SerializeField] private bool _autoDisableControlables = true;
        [SerializeField] private MonoBehaviour[] _componentsToDisable = Array.Empty<MonoBehaviour>();

        [Header("Events")]
        [SerializeField] private UnityEvent _onCharged;

        private Tween _moveTween;
        private Tween _rotateTween;
        private bool _isOccupied;
        private bool _isAttaching;

        public event Action<BatterySocketTrigger, IBattery> Charged;

        private void Awake()
        {
            if (_socketRoot == null)
            {
                _socketRoot = transform;
            }
        }

        private void OnDestroy()
        {
            _moveTween?.Kill();
            _rotateTween?.Kill();
            _moveTween = null;
            _rotateTween = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isAttaching || (_consumeOnce && _isOccupied) || other == null)
            {
                return;
            }

            if (!TryResolveBattery(other, out var battery, out var batteryComponent))
            {
                return;
            }

            _isAttaching = true;
            DisablePushPullInterfaces(batteryComponent);
            AttachBattery(battery, batteryComponent);
        }

        private void AttachBattery(IBattery battery, Component batteryComponent)
        {
            var batteryTransform = batteryComponent.transform;
            _moveTween?.Kill();
            _rotateTween?.Kill();

            if (_attachDuration <= 0f)
            {
                FinalizeAttach(battery, batteryTransform);
                return;
            }

            _moveTween = batteryTransform
                .DOMove(_socketRoot.position, _attachDuration)
                .SetEase(_attachEase);

            if (_matchSocketRotation)
            {
                _rotateTween = batteryTransform
                    .DORotateQuaternion(_socketRoot.rotation, _attachDuration)
                    .SetEase(_attachEase);
            }

            _moveTween.OnComplete(() => FinalizeAttach(battery, batteryTransform));
        }

        private void FinalizeAttach(IBattery battery, Transform batteryTransform)
        {
            batteryTransform.SetParent(_socketRoot, true);
            batteryTransform.position = _socketRoot.position;

            if (_matchSocketRotation)
            {
                batteryTransform.rotation = _socketRoot.rotation;
            }

            _isOccupied = true;
            _isAttaching = false;
            _moveTween = null;
            _rotateTween = null;

            _onCharged?.Invoke();
            Charged?.Invoke(this, battery);
        }

        private static bool TryResolveBattery(Collider other, out IBattery battery, out Component batteryComponent)
        {
            var behaviours = other.GetComponentsInParent<MonoBehaviour>(true);
            for (var i = 0; i < behaviours.Length; i++)
            {
                var behaviour = behaviours[i];
                if (behaviour is IBattery foundBattery)
                {
                    battery = foundBattery;
                    batteryComponent = behaviour;
                    return true;
                }
            }

            battery = null;
            batteryComponent = null;
            return false;
        }

        private void DisablePushPullInterfaces(Component batteryComponent)
        {
            for (var i = 0; i < _componentsToDisable.Length; i++)
            {
                if (_componentsToDisable[i] != null)
                {
                    _componentsToDisable[i].enabled = false;
                }
            }

            if (!_autoDisableControlables || batteryComponent == null)
            {
                return;
            }

            var controls = batteryComponent.GetComponentsInChildren<MonoBehaviour>(true);
            for (var i = 0; i < controls.Length; i++)
            {
                var behaviour = controls[i];
                if (behaviour is IControlable)
                {
                    behaviour.enabled = false;
                }
            }
        }
    }
}
