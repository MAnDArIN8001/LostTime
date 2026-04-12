using UnityEngine;

namespace Gameplay.Interaction.Core
{
    public interface IPressable
    {
        string PressPrompt { get; }

        bool CanPress(GameObject interactor, in PointerTargetContext pointerContext);

        void Press(GameObject interactor, in PointerTargetContext pointerContext);
    }
}
