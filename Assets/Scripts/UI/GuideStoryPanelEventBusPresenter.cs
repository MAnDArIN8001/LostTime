using System.Collections;
using Gameplay.Guide.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Events;

namespace UI
{
    [DisallowMultipleComponent]
    public sealed class GuideStoryPanelEventBusPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private GameObject _panelRoot;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _bodyLabel;
        [SerializeField] private Button _closeButton;

        [Header("Animation")]
        [SerializeField, Min(0f)] private float _fadeDuration = 0.2f;
        [SerializeField] private bool _useUnscaledTime = true;

        [Header("Behavior")]
        [SerializeField] private bool _hidePanelOnEnable = true;

        private EventBus _eventBus;
        private Coroutine _fadeCoroutine;
        private bool _isSubscribed;

        private void OnEnable()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(OnCloseButtonClicked);
            }

            if (_hidePanelOnEnable)
            {
                HideImmediate();
            }

            TrySubscribeToEventBus();
        }

        private void Update()
        {
            TrySubscribeToEventBus();
        }

        private void OnDisable()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            }

            UnsubscribeFromEventBus();
            StopFadeRoutine();
        }

        private void OnGuideStoryRequested(GuideStoryRequestedEvent eventData)
        {
            SetText(eventData.Metadata);
            ShowAnimated();
        }

        private void OnCloseButtonClicked()
        {
            HideAnimated();
        }

        private void SetText(GuideStoryMetadata metadata)
        {
            if (_titleLabel != null)
            {
                _titleLabel.text = metadata.Title;
            }

            if (_bodyLabel != null)
            {
                _bodyLabel.text = metadata.Text;
            }
        }

        private void ShowAnimated()
        {
            if (_panelRoot == null)
            {
                return;
            }

            _panelRoot.SetActive(true);

            if (!TryResolveCanvasGroup(out var canvasGroup) || _fadeDuration <= 0f)
            {
                SetCanvasState(canvasGroup, 1f, true);
                return;
            }

            StartFade(canvasGroup, canvasGroup.alpha, 1f, true, false);
        }

        private void HideAnimated()
        {
            if (_panelRoot == null || !_panelRoot.activeSelf)
            {
                return;
            }

            if (!TryResolveCanvasGroup(out var canvasGroup) || _fadeDuration <= 0f)
            {
                HideImmediate();
                return;
            }

            StartFade(canvasGroup, canvasGroup.alpha, 0f, false, true);
        }

        private void HideImmediate()
        {
            if (_panelRoot == null)
            {
                return;
            }

            if (TryResolveCanvasGroup(out var canvasGroup))
            {
                SetCanvasState(canvasGroup, 0f, false);
            }

            _panelRoot.SetActive(false);
        }

        private void StartFade(
            CanvasGroup canvasGroup,
            float fromAlpha,
            float toAlpha,
            bool interactableAfterFade,
            bool deactivatePanelAfterFade)
        {
            StopFadeRoutine();
            _fadeCoroutine = StartCoroutine(FadeRoutine(
                canvasGroup,
                fromAlpha,
                toAlpha,
                interactableAfterFade,
                deactivatePanelAfterFade));
        }

        private IEnumerator FadeRoutine(
            CanvasGroup canvasGroup,
            float fromAlpha,
            float toAlpha,
            bool interactableAfterFade,
            bool deactivatePanelAfterFade)
        {
            if (canvasGroup == null)
            {
                yield break;
            }

            SetCanvasState(canvasGroup, fromAlpha, false);

            var elapsed = 0f;
            var duration = Mathf.Max(0.0001f, _fadeDuration);

            while (elapsed < duration)
            {
                elapsed += _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                SetCanvasState(canvasGroup, alpha, false);
                yield return null;
            }

            SetCanvasState(canvasGroup, toAlpha, interactableAfterFade);

            if (deactivatePanelAfterFade && _panelRoot != null)
            {
                _panelRoot.SetActive(false);
            }

            _fadeCoroutine = null;
        }

        private void SetCanvasState(CanvasGroup canvasGroup, float alpha, bool interactable)
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = alpha;
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;
        }

        private void StopFadeRoutine()
        {
            if (_fadeCoroutine == null)
            {
                return;
            }

            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        private bool TryResolveCanvasGroup(out CanvasGroup canvasGroup)
        {
            if (_canvasGroup != null)
            {
                canvasGroup = _canvasGroup;
                return true;
            }

            if (_panelRoot != null)
            {
                _canvasGroup = _panelRoot.GetComponent<CanvasGroup>();
            }

            canvasGroup = _canvasGroup;
            return canvasGroup != null;
        }

        private bool TryResolveEventBus(out EventBus eventBus)
        {
            if (_eventBusProvider != null && _eventBusProvider.EventBus != null)
            {
                eventBus = _eventBusProvider.EventBus;
                return true;
            }

            return SceneEventBusProvider.TryGetEventBus(out eventBus);
        }

        private void TrySubscribeToEventBus()
        {
            if (_isSubscribed)
            {
                return;
            }

            if (!TryResolveEventBus(out _eventBus))
            {
                return;
            }

            _eventBus.Subscribe<GuideStoryRequestedEvent>(OnGuideStoryRequested);
            _isSubscribed = true;
        }

        private void UnsubscribeFromEventBus()
        {
            if (!_isSubscribed || _eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<GuideStoryRequestedEvent>(OnGuideStoryRequested);
            _eventBus = null;
            _isSubscribed = false;
        }
    }
}
