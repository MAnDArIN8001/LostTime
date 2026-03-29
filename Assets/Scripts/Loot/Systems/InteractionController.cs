using System;
using System.Linq;
using UnityEngine;
using Utils.Filters;
using Utils.Physics.Raycaster;

namespace Loot.Systems
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private DirectionalRaycaster _directionalRaycaster;
        [SerializeField, Min(1)] private int _framesWithoutHitBeforeClear = 2;

        private IRaycastFilter _raycastFilter;
        private IMarkable _currentMarkable;
        private IInteractable _currentInteractable;
        private ITakable _currentTakable;
        private int _lastProcessedHitFrame = -1;

        public string CurrentInteractHint => _currentInteractable?.CanInteract == true
            ? _currentInteractable.InteractionPrompt
            : string.Empty;

        public event Action<string> FocusHintChanged;
        public event Action<ITakable, GameObject> PickupCollected;

        private void OnEnable()
        {
            if (_directionalRaycaster == null)
            {
                return;
            }

            _raycastFilter = new RaycastFilter(_directionalRaycaster, hit => hit.collider != null);
            _raycastFilter.OnHitProcessed += ProcessHits;
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

            ClearFocus();
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
            if (_currentInteractable != null && _currentInteractable.CanInteract)
            {
                _currentInteractable.Interact(interactor);
                NotifyHintChanged();
                return true;
            }

            if (_currentTakable == null)
            {
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

                return collected;
            }

            var currentTakable = _currentTakable;
            PickupCollected?.Invoke(currentTakable, interactor);
            _currentTakable.Take();
            return true;
        }

        private void ProcessHits(RaycastHit[] hits)
        {
            _lastProcessedHitFrame = Time.frameCount;

            if (hits == null || hits.Length == 0)
            {
                ClearFocus();
                return;
            }

            var bestHit = hits
                .OrderBy(hit => hit.distance)
                .FirstOrDefault(hit => TryResolveTarget(hit.collider, out _, out _, out _));

            if (bestHit.collider == null ||
                !TryResolveTarget(bestHit.collider, out var markable, out var interactable, out var takable))
            {
                ClearFocus();
                return;
            }

            SetFocus(markable, interactable, takable);
        }

        private void SetFocus(IMarkable markable, IInteractable interactable, ITakable takable)
        {
            if (ReferenceEquals(_currentMarkable, markable) &&
                ReferenceEquals(_currentInteractable, interactable) &&
                ReferenceEquals(_currentTakable, takable))
            {
                return;
            }

            _currentMarkable?.HideMark();
            _currentMarkable = markable;
            _currentInteractable = interactable;
            _currentTakable = takable;
            _currentMarkable?.ShowMark();

            NotifyHintChanged();
        }

        private void ClearFocus()
        {
            _lastProcessedHitFrame = -1;
            _currentMarkable?.HideMark();
            _currentMarkable = null;
            _currentInteractable = null;
            _currentTakable = null;
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

            FocusHintChanged?.Invoke(hint);
        }

        private static bool TryResolveTarget(
            Collider collider,
            out IMarkable markable,
            out IInteractable interactable,
            out ITakable takable)
        {
            markable = collider.GetComponentInParent<IMarkable>();
            interactable = collider.GetComponentInParent<IInteractable>();
            takable = collider.GetComponentInParent<ITakable>();

            return markable != null || interactable != null || takable != null;
        }
    }
}
