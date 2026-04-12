using UnityEngine;

namespace Gameplay.Interaction.Core
{
    public readonly struct ControlSessionSnapshot
    {
        public ControlSessionSnapshot(
            int version,
            IControlable target,
            GameObject interactor,
            ControlMode mode,
            PointerTargetContext pointerContext)
        {
            Version = version;
            Target = target;
            Interactor = interactor;
            Mode = mode;
            PointerContext = pointerContext;
        }

        public int Version { get; }
        public IControlable Target { get; }
        public GameObject Interactor { get; }
        public ControlMode Mode { get; }
        public PointerTargetContext PointerContext { get; }

        public bool IsActive => Target != null && Interactor != null && Mode != ControlMode.None;

        public static ControlSessionSnapshot Inactive(int version)
        {
            return new ControlSessionSnapshot(
                version,
                null,
                null,
                ControlMode.None,
                default);
        }
    }
}
