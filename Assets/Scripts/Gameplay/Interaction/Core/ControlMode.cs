using System;

namespace Gameplay.Interaction.Core
{
    [Flags]
    public enum ControlMode
    {
        None = 0,
        Push = 1 << 0,
        Pull = 1 << 1,
    }
}
