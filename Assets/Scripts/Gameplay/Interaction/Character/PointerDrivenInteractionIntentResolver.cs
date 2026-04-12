using Gameplay.Input;
using Gameplay.Interaction.Core;
using UnityEngine;

namespace Gameplay.Interaction.Character
{
    public sealed class PointerDrivenInteractionIntentResolver
    {
        private readonly IPointerControlSignalSource _pointerControlSignalSource;

        public PointerDrivenInteractionIntentResolver(IPointerControlSignalSource pointerControlSignalSource)
        {
            _pointerControlSignalSource = pointerControlSignalSource;
        }

        public InteractionIntentResolution Resolve(bool interactionRequested, in InteractionFocusContext focusContext)
        {
            if (!interactionRequested)
            {
                return InteractionIntentResolution.None;
            }

            // Keep existing interact/take path as a hard fallback.
            if (focusContext.HasPressable || focusContext.HasInteractable || focusContext.HasTakable)
            {
                InteractionDebugLog.LogVerbose(
                    focusContext.PointerContext.HitCollider,
                    $"Intent resolved to Press. pressable={focusContext.HasPressable}, interactable={focusContext.HasInteractable}, takable={focusContext.HasTakable}.");
                return InteractionIntentResolution.Press;
            }

            if (!TryResolveControlTarget(focusContext, out var controlTarget))
            {
                InteractionDebugLog.LogVerbose(
                    focusContext.PointerContext.HitCollider,
                    "Intent resolved to Press fallback: no control target on focused object.");
                return InteractionIntentResolution.Press;
            }

            if (_pointerControlSignalSource == null ||
                !_pointerControlSignalSource.TryReadControlAxis(out var controlAxis))
            {
                InteractionDebugLog.LogVerbose(
                    focusContext.PointerContext.HitCollider,
                    $"Intent resolved to PressWithControlCandidate for '{ResolveTargetName(focusContext)}': no control axis signal.");
                return InteractionIntentResolution.PressWithControlCandidate(controlTarget);
            }

            if (!TryResolveControlMode(controlAxis, controlTarget.SupportedModes, out var controlMode))
            {
                InteractionDebugLog.LogVerbose(
                    focusContext.PointerContext.HitCollider,
                    $"Intent resolved to PressWithControlCandidate for '{ResolveTargetName(focusContext)}': axis={controlAxis:0.###} not supported by {controlTarget.SupportedModes}.");
                return InteractionIntentResolution.PressWithControlCandidate(controlTarget);
            }

            InteractionDebugLog.LogVerbose(
                focusContext.PointerContext.HitCollider,
                $"Intent resolved to {controlMode} for '{ResolveTargetName(focusContext)}' with axis={controlAxis:0.###}.");
            return InteractionIntentResolution.Control(controlTarget, controlMode);
        }

        private static bool TryResolveControlTarget(in InteractionFocusContext focusContext, out IControlable controlTarget)
        {
            controlTarget = null;

            if (!focusContext.HasFocusTarget)
            {
                return false;
            }

            var pointerContext = focusContext.PointerContext;
            var target = pointerContext.Target;
            if (target == null && pointerContext.HitCollider != null)
            {
                target = pointerContext.HitCollider.gameObject;
            }

            if (target == null)
            {
                return false;
            }

            if (!target.TryGetComponent(typeof(IControlable), out var resolvedComponent))
            {
                return false;
            }

            controlTarget = resolvedComponent as IControlable;
            return controlTarget != null;
        }

        private static bool TryResolveControlMode(float controlAxis, ControlMode supportedModes, out ControlMode controlMode)
        {
            controlMode = ControlMode.None;

            if (Mathf.Approximately(controlAxis, 0f))
            {
                return false;
            }

            var preferredMode = controlAxis > 0f ? ControlMode.Push : ControlMode.Pull;
            if ((supportedModes & preferredMode) == preferredMode)
            {
                controlMode = preferredMode;
                return true;
            }

            return false;
        }

        private static string ResolveTargetName(in InteractionFocusContext focusContext)
        {
            var pointerContext = focusContext.PointerContext;
            if (pointerContext.Target != null)
            {
                return pointerContext.Target.name;
            }

            return pointerContext.HitCollider != null
                ? pointerContext.HitCollider.name
                : "null";
        }
    }
}
