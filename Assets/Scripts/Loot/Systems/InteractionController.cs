using System;
using Gameplay.Interaction.Core;
using UnityEngine;
using Utils.Filters;
using Utils.Physics.Raycaster;

namespace Loot.Systems
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private DirectionalRaycaster _directionalRaycaster;
        [SerializeField, Min(1)] private int _framesWithoutHitBeforeClear = 2;
        [SerializeField] private bool _enableDebugLogs;
        [SerializeField] private bool _verboseDiscoveryLogs;

        private IRaycastFilter _raycastFilter;
        private readonly LootInteractionFocusDiscovery _focusDiscovery = new();
        private IMarkable _currentMarkable;
        private IPressable _currentPressable;
        private IInteractable _currentInteractable;
        private ITakable _currentTakable;
        private IControlable _currentControlable;
        private InteractionFocusContext _currentFocusContext;
        private int _lastProcessedHitFrame = -1;

        public string CurrentInteractHint
        {
            get
            {
                if (_currentPressable != null && !string.IsNullOrWhiteSpace(_currentPressable.PressPrompt))
                {
                    return _currentPressable.PressPrompt;
                }

                if (_currentInteractable?.CanInteract == true)
                {
                    return _currentInteractable.InteractionPrompt;
                }

                return string.Empty;
            }
        }
        public InteractionFocusContext CurrentFocusContext => _currentFocusContext;
        public bool DebugLoggingEnabled => _enableDebugLogs;

        public event Action<string> FocusHintChanged;
        public event Action<ITakable, GameObject> PickupCollected;

        private void OnEnable()
        {
            InteractionDebugLog.Configure(_enableDebugLogs, _verboseDiscoveryLogs);

            if (_directionalRaycaster == null)
            {
                InteractionDebugLog.Log(this, "InteractionController enabled without DirectionalRaycaster reference.");
                return;
            }

            _raycastFilter = new RaycastFilter(_directionalRaycaster, hit => hit.collider != null);
            _raycastFilter.OnHitProcessed += ProcessHits;
            InteractionDebugLog.Log(this, "InteractionController enabled and subscribed to directional raycaster.");
            NotifyHintChanged();
        }

        private void OnDisable()
        {
            if (_raycastFilter != null)
            {
                _raycastFilter.OnHitProcessed -= ProcessHits;
                _raycastFilter.Dispose();
                _raycastFilter = null;
            }

            InteractionDebugLog.Log(this, "InteractionController disabled. Clearing focus state.");
            ClearFocus();
        }

        private void OnValidate()
        {
            InteractionDebugLog.Configure(_enableDebugLogs, _verboseDiscoveryLogs);
        }

        private void Update()
        {
            if (_lastProcessedHitFrame < 0)
            {
                return;
            }

            if (Time.frameCount > _lastProcessedHitFrame + _framesWithoutHitBeforeClear)
            {
                ClearFocus();
            }
        }

        public bool TryInteract(GameObject interactor)
        {
            var pointerContext = _currentFocusContext.PointerContext;
            InteractionDebugLog.Log(
                this,
                $"TryInteract requested by '{(interactor != null ? interactor.name : "null")}'. " +
                $"focusTarget='{ResolveFocusTargetName()}', pressable={_currentPressable != null}, interactable={_currentInteractable != null}, takable={_currentTakable != null}, controlable={_currentControlable != null}.");

            if (_currentPressable != null)
            {
                if (_currentPressable.CanPress(interactor, pointerContext))
                {
                    _currentPressable.Press(interactor, pointerContext);
                    InteractionDebugLog.Log(this, $"Pressable interaction executed on '{ResolveFocusTargetName()}'.");
                    NotifyHintChanged();
                    return true;
                }

                InteractionDebugLog.Log(this, $"Pressable interaction rejected by target '{ResolveFocusTargetName()}'.");
            }

            if (_currentInteractable != null && _currentInteractable.CanInteract)
            {
                _currentInteractable.Interact(interactor);
                InteractionDebugLog.Log(this, $"Legacy interactable interaction executed on '{ResolveFocusTargetName()}'.");
                NotifyHintChanged();
                return true;
            }

            if (_currentTakable == null)
            {
                InteractionDebugLog.Log(this, "TryInteract finished without action: no interactable, pressable, or takable target.");
                return false;
            }

            if (_currentTakable is ICollectible collectible)
            {
                var takable = _currentTakable;
                var collected = collectible.TryCollect(interactor);
                if (collected)
                {
                    PickupCollected?.Invoke(takable, interactor);
                }

                InteractionDebugLog.Log(this, $"Collectible interaction on '{ResolveFocusTargetName()}' returned {collected}.");
                return collected;
            }

            var currentTakable = _currentTakable;
            PickupCollected?.Invoke(currentTakable, interactor);
            _currentTakable.Take();
            InteractionDebugLog.Log(this, $"Takable interaction executed on '{ResolveFocusTargetName()}'.");
            return true;
        }

        private void ProcessHits(RaycastHit[] hits)
        {
            _lastProcessedHitFrame = Time.frameCount;
            InteractionDebugLog.LogVerbose(this, $"ProcessHits received {hits?.Length ?? 0} hits on frame {Time.frameCount}.");

            if (!_focusDiscovery.TryDiscover(hits, out var focus))
            {
                InteractionDebugLog.Log(this, "ProcessHits found no valid interaction focus. Clearing current focus.");
                ClearFocus();
                return;
            }

            SetFocus(focus);
        }

        private void SetFocus(in LootInteractionFocus focus)
        {
            if (ReferenceEquals(_currentMarkable, focus.Markable) &&
                ReferenceEquals(_currentPressable, focus.Pressable) &&
                ReferenceEquals(_currentInteractable, focus.Interactable) &&
                ReferenceEquals(_currentTakable, focus.Takable) &&
                ReferenceEquals(_currentControlable, focus.Controlable))
            {
                return;
            }

            _currentMarkable?.HideMark();
            _currentMarkable = focus.Markable;
            _currentPressable = focus.Pressable;
            _currentInteractable = focus.Interactable;
            _currentTakable = focus.Takable;
            _currentControlable = focus.Controlable;
            _currentFocusContext = focus.Context;
            _currentMarkable?.ShowMark();

            InteractionDebugLog.Log(
                this,
                $"Focus set to '{ResolveFocusTargetName()}'. markable={_currentMarkable != null}, pressable={_currentPressable != null}, interactable={_currentInteractable != null}, takable={_currentTakable != null}, controlable={_currentControlable != null}, distance={_currentFocusContext.PointerContext.Distance:0.###}.");

            NotifyHintChanged();
        }

        private void ClearFocus()
        {
            var hadFocus = _currentMarkable != null || _currentPressable != null || _currentInteractable != null || _currentTakable != null || _currentControlable != null;
            _lastProcessedHitFrame = -1;
            _currentMarkable?.HideMark();
            _currentMarkable = null;
            _currentPressable = null;
            _currentInteractable = null;
            _currentTakable = null;
            _currentControlable = null;
            _currentFocusContext = default;
            if (hadFocus)
            {
                InteractionDebugLog.Log(this, "Focus cleared.");
            }
            NotifyHintChanged();
        }

        private void NotifyHintChanged()
        {
            var hint = CurrentInteractHint;

            if (string.IsNullOrWhiteSpace(hint) && _currentTakable != null)
            {
                hint = string.IsNullOrWhiteSpace(_currentTakable.InteractionPrompt)
                    ? "Take"
                    : _currentTakable.InteractionPrompt;
            }

            InteractionDebugLog.LogVerbose(this, $"Interaction hint changed to '{hint}'.");
            FocusHintChanged?.Invoke(hint);
        }

        private string ResolveFocusTargetName()
        {
            var pointerContext = _currentFocusContext.PointerContext;
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
