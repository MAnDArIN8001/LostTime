using Gameplay.Interaction.Character;
using Gameplay.Interaction.Core;
using UnityEngine;

namespace Gameplay.Interaction.Authoring
{
    [DisallowMultipleComponent]
    public sealed class InteractionRuntimeDiagnosticsGizmo : MonoBehaviour
    {
        [Header("Runtime Snapshot")]
        [SerializeField] private bool _hasRuntimeSnapshot;
        [SerializeField] private int _snapshotFrame = -1;
        [SerializeField] private bool _interactionRequested;
        [SerializeField] private bool _interactionHandled;
        [SerializeField] private InteractionIntent _intent;
        [SerializeField] private bool _hasFocusTarget;
        [SerializeField] private bool _controlSessionActive;
        [SerializeField] private CharacterControlSessionBlockReason _controlBlockReason;
        [SerializeField, TextArea(3, 10)] private string _runtimeSummary;

        [Header("Gizmo Options")]
        [SerializeField] private bool _drawFocusTarget = true;
        [SerializeField] private bool _drawPointerHit = true;
        [SerializeField] private bool _drawControlSession = true;
        [SerializeField, Min(0.01f)] private float _focusRadius = 0.2f;
        [SerializeField, Min(0.01f)] private float _pointerRadius = 0.08f;
        [SerializeField, Min(0.01f)] private float _normalLength = 0.4f;
        [SerializeField] private Color _focusColor = new Color(1f, 0.95f, 0.25f, 0.9f);
        [SerializeField] private Color _pointerColor = new Color(1f, 0.6f, 0.1f, 0.9f);
        [SerializeField] private Color _controlSessionColor = new Color(0.2f, 0.95f, 1f, 0.9f);

        private int _observedVersion = -1;

        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            RefreshSnapshotFromRuntime();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            RefreshSnapshotFromRuntime();
            DrawGizmos(CharacterInteractionDiagnostics.Latest);
        }

        private void RefreshSnapshotFromRuntime()
        {
            if (_observedVersion == CharacterInteractionDiagnostics.Version)
            {
                return;
            }

            _observedVersion = CharacterInteractionDiagnostics.Version;
            var snapshot = CharacterInteractionDiagnostics.Latest;

            _hasRuntimeSnapshot = CharacterInteractionDiagnostics.Version > 0;
            _snapshotFrame = snapshot.Frame;
            _interactionRequested = snapshot.InteractionRequested;
            _interactionHandled = snapshot.InteractionHandled;
            _intent = snapshot.Resolution.Intent;
            _hasFocusTarget = snapshot.FocusContext.HasFocusTarget;
            _controlSessionActive = snapshot.ControlSessionSnapshot.IsActive;
            _controlBlockReason = snapshot.PolicyDecision.BlockReason;
            _runtimeSummary = BuildSummary(snapshot);
        }

        private void DrawGizmos(in CharacterInteractionDiagnosticsSnapshot snapshot)
        {
            if (!_hasRuntimeSnapshot)
            {
                return;
            }

            var pointerContext = snapshot.FocusContext.PointerContext;

            if (_drawFocusTarget &&
                snapshot.FocusContext.HasFocusTarget &&
                TryResolveTargetPosition(pointerContext, out var focusPosition))
            {
                Gizmos.color = _focusColor;
                Gizmos.DrawWireSphere(focusPosition, _focusRadius);
            }

            if (_drawPointerHit && pointerContext.HasTarget)
            {
                Gizmos.color = _pointerColor;
                Gizmos.DrawSphere(pointerContext.HitPoint, _pointerRadius);
                Gizmos.DrawLine(pointerContext.HitPoint, pointerContext.HitPoint + pointerContext.HitNormal * _normalLength);
            }

            if (!_drawControlSession || !snapshot.ControlSessionSnapshot.IsActive)
            {
                return;
            }

            var controlSnapshot = snapshot.ControlSessionSnapshot;
            var interactorPosition = controlSnapshot.Interactor != null
                ? controlSnapshot.Interactor.transform.position
                : controlSnapshot.PointerContext.HitPoint;
            var targetPosition = TryResolveTargetPosition(controlSnapshot.PointerContext, out var controlTargetPosition)
                ? controlTargetPosition
                : controlSnapshot.PointerContext.HitPoint;

            Gizmos.color = _controlSessionColor;
            Gizmos.DrawLine(interactorPosition, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, _focusRadius * 0.85f);
            Gizmos.DrawSphere(controlSnapshot.PointerContext.HitPoint, _pointerRadius * 0.8f);
        }

        private static bool TryResolveTargetPosition(in PointerTargetContext pointerContext, out Vector3 position)
        {
            if (pointerContext.Target != null)
            {
                position = pointerContext.Target.transform.position;
                return true;
            }

            if (pointerContext.HitCollider != null)
            {
                position = pointerContext.HitCollider.bounds.center;
                return true;
            }

            position = pointerContext.HitPoint;
            return pointerContext.HasTarget;
        }

        private static string BuildSummary(in CharacterInteractionDiagnosticsSnapshot snapshot)
        {
            var focus = snapshot.FocusContext;
            var pointer = focus.PointerContext;
            var controlSnapshot = snapshot.ControlSessionSnapshot;

            var controlTargetName = "None";
            if (snapshot.Resolution.ControlTarget is Component resolutionTargetComponent)
            {
                controlTargetName = resolutionTargetComponent.name;
            }
            else if (controlSnapshot.Target is Component sessionTargetComponent)
            {
                controlTargetName = sessionTargetComponent.name;
            }

            return
                $"Frame: {snapshot.Frame}\n" +
                $"Intent: {snapshot.Resolution.Intent} (requested={snapshot.InteractionRequested}, handled={snapshot.InteractionHandled})\n" +
                $"Focus: hasTarget={focus.HasFocusTarget}, markable={focus.HasMarkable}, pressable={focus.HasPressable}, interactable={focus.HasInteractable}, takable={focus.HasTakable}, controlable={focus.HasControlable}\n" +
                $"Pointer: hasTarget={pointer.HasTarget}, distance={pointer.Distance:F2}\n" +
                $"Control Resolution: target={controlTargetName}, mode={snapshot.Resolution.ControlMode}\n" +
                $"Control Session: active={controlSnapshot.IsActive}, mode={controlSnapshot.Mode}, version={controlSnapshot.Version}\n" +
                $"Policy: allow={snapshot.PolicyDecision.AllowControlSession}, moveParallel={snapshot.PolicyDecision.AllowParallelMovement}, block={snapshot.PolicyDecision.BlockReason}";
        }
    }
}
