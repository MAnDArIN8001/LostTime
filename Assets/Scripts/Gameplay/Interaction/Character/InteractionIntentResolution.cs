using Gameplay.Interaction.Core;

namespace Gameplay.Interaction.Character
{
    public readonly struct InteractionIntentResolution
    {
        public InteractionIntentResolution(InteractionIntent intent, IControlable controlTarget, ControlMode controlMode)
        {
            Intent = intent;
            ControlTarget = controlTarget;
            ControlMode = controlMode;
        }

        public InteractionIntent Intent { get; }
        public IControlable ControlTarget { get; }
        public ControlMode ControlMode { get; }

        public bool HasControlCandidate => ControlTarget != null;

        public static InteractionIntentResolution None => new(InteractionIntent.None, null, ControlMode.None);
        public static InteractionIntentResolution Press => new(InteractionIntent.Press, null, ControlMode.None);

        public static InteractionIntentResolution PressWithControlCandidate(IControlable controlTarget)
        {
            return new InteractionIntentResolution(InteractionIntent.Press, controlTarget, ControlMode.None);
        }

        public static InteractionIntentResolution Control(IControlable controlTarget, ControlMode controlMode)
        {
            var intent = controlMode == ControlMode.Push
                ? InteractionIntent.Push
                : InteractionIntent.Pull;

            return new InteractionIntentResolution(intent, controlTarget, controlMode);
        }
    }
}
