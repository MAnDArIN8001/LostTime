using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Events;

namespace Gameplay.Input
{
    [DisallowMultipleComponent]
    public sealed class ActiveInputTypeEventPublisher : MonoBehaviour
    {
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private ActiveInputType _fallbackInputType = ActiveInputType.KeyboardAndMouse;

        private ActiveInputType _currentInputType = ActiveInputType.Unknown;

        private void OnEnable()
        {
            PublishIfChanged(ResolveCurrentInputType(true));
        }

        private void Update()
        {
            PublishIfChanged(ResolveCurrentInputType(false));
        }

        private ActiveInputType ResolveCurrentInputType(bool allowFallback)
        {
            if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
            {
                return ActiveInputType.Gamepad;
            }

            if (Touchscreen.current != null && Touchscreen.current.wasUpdatedThisFrame)
            {
                return ActiveInputType.Touch;
            }

            if ((Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame) ||
                (Mouse.current != null && Mouse.current.wasUpdatedThisFrame))
            {
                return ActiveInputType.KeyboardAndMouse;
            }

            if (!allowFallback && _currentInputType != ActiveInputType.Unknown)
            {
                return _currentInputType;
            }

            return _fallbackInputType;
        }

        private void PublishIfChanged(ActiveInputType nextInputType)
        {
            if (nextInputType == ActiveInputType.Unknown || nextInputType == _currentInputType)
            {
                return;
            }

            _currentInputType = nextInputType;

            if (TryResolveEventBus(out var eventBus))
            {
                eventBus.Publish(new ActiveInputTypeChangedEvent(nextInputType));
            }
        }

        private bool TryResolveEventBus(out EventBus eventBus)
        {
            if (_eventBusProvider != null && _eventBusProvider.EventBus != null)
            {
                eventBus = _eventBusProvider.EventBus;
                return true;
            }

            return SceneEventBusProvider.TryGetEventBus(out eventBus);
        }
    }
}
