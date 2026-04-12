using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Input
{
    public sealed class MouseScrollPointerControlSignalSource : IPointerControlSignalSource
    {
        private readonly float _deadZone;

        public MouseScrollPointerControlSignalSource(float deadZone = 0.01f)
        {
            _deadZone = Mathf.Max(0f, deadZone);
        }

        public bool TryReadControlAxis(out float axisValue)
        {
            axisValue = 0f;
            var mouse = Mouse.current;

            if (mouse == null)
            {
                return false;
            }

            var scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) <= _deadZone)
            {
                return false;
            }

            axisValue = scroll;
            return true;
        }
    }
}
