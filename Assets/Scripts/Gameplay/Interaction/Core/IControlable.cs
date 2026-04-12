using UnityEngine;

namespace Gameplay.Interaction.Core
{
    public interface IControlable
    {
        string ControlPrompt { get; }
        ControlMode SupportedModes { get; }

        bool CanControl(ControlMode mode, GameObject interactor, in PointerTargetContext pointerContext);

        void BeginControl(ControlMode mode, GameObject interactor, in PointerTargetContext pointerContext);

        void UpdateControl(ControlMode mode, GameObject interactor, in PointerTargetContext pointerContext);

        void EndControl(ControlMode mode, GameObject interactor, in PointerTargetContext pointerContext);
    }
}
