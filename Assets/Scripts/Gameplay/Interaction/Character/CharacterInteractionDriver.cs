using Gameplay.Interaction.Core;
using Gameplay.Input;
using Loot.Systems;
using UnityEngine;

namespace Gameplay.Interaction.Character
{
    public sealed class CharacterInteractionDriver
    {
        private readonly InteractionController _interactionController;
        private readonly PointerDrivenInteractionIntentResolver _intentResolver;
        private readonly ICharacterControlSessionPolicy _controlSessionPolicy;
        private readonly ControlSession _controlSession;

        public CharacterInteractionDriver(InteractionController interactionController)
            : this(
                interactionController,
                new PointerDrivenInteractionIntentResolver(new MouseScrollPointerControlSignalSource()),
                new DefaultCharacterControlSessionPolicy(),
                new ControlSession())
        {
        }

        internal CharacterInteractionDriver(
            InteractionController interactionController,
            PointerDrivenInteractionIntentResolver intentResolver,
            ICharacterControlSessionPolicy controlSessionPolicy,
            ControlSession controlSession)
        {
            _interactionController = interactionController;
            _intentResolver = intentResolver;
            _controlSessionPolicy = controlSessionPolicy;
            _controlSession = controlSession ?? new ControlSession();
        }

        public InteractionIntent CurrentIntent { get; private set; }
        public InteractionIntentResolution CurrentResolution { get; private set; }
        public CharacterControlSessionPolicyDecision CurrentControlPolicyDecision { get; private set; } =
            CharacterControlSessionPolicyDecision.Allowed;
        public bool IsControlSessionActive => _controlSession.IsActive;
        public ControlSessionSnapshot CurrentControlSessionSnapshot => _controlSession.Snapshot;

        public bool Tick(in CharacterInteractionFrameInput frameInput)
        {
            var focusContext = _interactionController != null
                ? _interactionController.CurrentFocusContext
                : default;
            var pointerContext = focusContext.PointerContext;

            CurrentResolution = _intentResolver != null
                ? _intentResolver.Resolve(frameInput.InteractionRequested, focusContext)
                : (frameInput.InteractionRequested ? InteractionIntentResolution.Press : InteractionIntentResolution.None);
            CurrentIntent = CurrentResolution.Intent;
            CurrentControlPolicyDecision = EvaluateControlPolicy(frameInput);

            if (frameInput.InteractionRequested)
            {
                InteractionDebugLog.Log(
                    focusContext.PointerContext.HitCollider,
                    $"Interaction tick: requested by '{(frameInput.Interactor != null ? frameInput.Interactor.name : "null")}', " +
                    $"intent={CurrentIntent}, policyAllow={CurrentControlPolicyDecision.AllowControlSession}, focusTarget='{ResolveTargetName(focusContext)}'.");
            }

            if (_controlSession.IsActive)
            {
                if (!CurrentControlPolicyDecision.AllowControlSession)
                {
                    StopActiveControlSession(pointerContext);
                }
                else
                {
                    MaintainActiveControlSession(pointerContext);
                }
            }

            var handled = false;
            if (frameInput.InteractionRequested && frameInput.Interactor != null && _interactionController != null)
            {
                handled = TryHandleIntent(
                    CurrentResolution,
                    frameInput.Interactor,
                    pointerContext,
                    CurrentControlPolicyDecision);
            }

            PublishDiagnostics(frameInput.InteractionRequested, handled, focusContext);
            return handled;
        }

        public bool TryHandleIntent(InteractionIntent intent, GameObject interactor)
        {
            var resolution = intent switch
            {
                InteractionIntent.Press => InteractionIntentResolution.Press,
                InteractionIntent.Push => new InteractionIntentResolution(InteractionIntent.Push, null, ControlMode.Push),
                InteractionIntent.Pull => new InteractionIntentResolution(InteractionIntent.Pull, null, ControlMode.Pull),
                _ => InteractionIntentResolution.None,
            };

            var pointerContext = _interactionController != null
                ? _interactionController.CurrentFocusContext.PointerContext
                : default;

            return TryHandleIntent(
                resolution,
                interactor,
                pointerContext,
                CurrentControlPolicyDecision);
        }

        public bool TryHandleIntent(in InteractionIntentResolution resolution, GameObject interactor)
        {
            var pointerContext = _interactionController != null
                ? _interactionController.CurrentFocusContext.PointerContext
                : default;

            return TryHandleIntent(
                resolution,
                interactor,
                pointerContext,
                CurrentControlPolicyDecision);
        }

        private CharacterControlSessionPolicyDecision EvaluateControlPolicy(in CharacterInteractionFrameInput frameInput)
        {
            if (_controlSessionPolicy == null)
            {
                return CharacterControlSessionPolicyDecision.Allowed;
            }

            var policyContext = new CharacterControlSessionPolicyContext(
                _controlSession.IsActive,
                frameInput.HasMovementInput,
                frameInput.IsAimPressed,
                frameInput.CastRequestedThisFrame);

            return _controlSessionPolicy.Evaluate(policyContext);
        }

        private bool TryHandleIntent(
            in InteractionIntentResolution resolution,
            GameObject interactor,
            in PointerTargetContext pointerContext,
            in CharacterControlSessionPolicyDecision policyDecision)
        {
            if (_interactionController == null || interactor == null)
            {
                InteractionDebugLog.Log(pointerContext.HitCollider, "TryHandleIntent aborted: interaction controller or interactor is missing.");
                return false;
            }

            if (resolution.Intent == InteractionIntent.Press)
            {
                if (_controlSession.IsActive)
                {
                    InteractionDebugLog.Log(pointerContext.HitCollider, "Press intent ignored because a control session is already active.");
                    return false;
                }

                InteractionDebugLog.Log(pointerContext.HitCollider, $"Handling Press intent for '{ResolveTargetName(pointerContext)}'.");
                return _interactionController.TryInteract(interactor);
            }

            if (resolution.Intent == InteractionIntent.Push ||
                resolution.Intent == InteractionIntent.Pull)
            {
                if (!policyDecision.AllowControlSession)
                {
                    InteractionDebugLog.Log(pointerContext.HitCollider, $"Control intent blocked by policy. reason={policyDecision.BlockReason}.");
                    return false;
                }

                if (resolution.ControlTarget == null || resolution.ControlMode == ControlMode.None)
                {
                    InteractionDebugLog.Log(pointerContext.HitCollider, "Control intent aborted: resolution does not contain a valid control target or mode.");
                    return false;
                }

                var resolvedPointerContext = ResolvePointerContext(pointerContext);

                if (_controlSession.IsActive)
                {
                    InteractionDebugLog.LogVerbose(resolvedPointerContext.HitCollider, $"Maintaining active control session on '{ResolveTargetName(resolvedPointerContext)}'.");
                    return _controlSession.TryMaintain(resolvedPointerContext);
                }

                if (!_controlSession.TryBegin(
                        resolution.ControlTarget,
                        resolution.ControlMode,
                        interactor,
                        resolvedPointerContext))
                {
                    InteractionDebugLog.Log(resolvedPointerContext.HitCollider, $"Failed to begin control session on '{ResolveTargetName(resolvedPointerContext)}' with mode={resolution.ControlMode}.");
                    return false;
                }

                return _controlSession.TryMaintain(resolvedPointerContext);
            }

            InteractionDebugLog.LogVerbose(pointerContext.HitCollider, $"No handler for interaction intent {resolution.Intent}.");
            return false;
        }

        private void MaintainActiveControlSession(in PointerTargetContext pointerContext)
        {
            var resolvedPointerContext = ResolvePointerContext(pointerContext);
            if (_controlSession.TryMaintain(resolvedPointerContext))
            {
                return;
            }

            InteractionDebugLog.Log(resolvedPointerContext.HitCollider, $"Active control session can no longer be maintained on '{ResolveTargetName(resolvedPointerContext)}'. Stopping it.");
            StopActiveControlSession(resolvedPointerContext);
        }

        private void StopActiveControlSession(in PointerTargetContext pointerContext)
        {
            if (!_controlSession.IsActive)
            {
                return;
            }

            var resolvedPointerContext = ResolvePointerContext(pointerContext);
            if (_controlSession.TryEnd(resolvedPointerContext))
            {
                return;
            }

            InteractionDebugLog.Log(resolvedPointerContext.HitCollider, $"Graceful control session end failed on '{ResolveTargetName(resolvedPointerContext)}'. Cancelling session.");
            _controlSession.Cancel();
        }

        private PointerTargetContext ResolvePointerContext(in PointerTargetContext pointerContext)
        {
            if (pointerContext.HasTarget)
            {
                return pointerContext;
            }

            if (_controlSession.IsActive)
            {
                return _controlSession.Snapshot.PointerContext;
            }

            return pointerContext;
        }

        private void PublishDiagnostics(
            bool interactionRequested,
            bool interactionHandled,
            in InteractionFocusContext focusContext)
        {
            CharacterInteractionDiagnostics.Publish(new CharacterInteractionDiagnosticsSnapshot(
                Time.frameCount,
                interactionRequested,
                interactionHandled,
                focusContext,
                CurrentResolution,
                CurrentControlPolicyDecision,
                _controlSession.Snapshot));
        }

        private static string ResolveTargetName(in InteractionFocusContext focusContext)
        {
            return ResolveTargetName(focusContext.PointerContext);
        }

        private static string ResolveTargetName(in PointerTargetContext pointerContext)
        {
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
