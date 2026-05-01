using Gameplay.Input;
using Gameplay.Interaction.Core;
using UI.Runtime;
using UnityEngine;
using Utils.Events;
using Zenject;

namespace UI
{
    [DisallowMultipleComponent]
    public sealed class InteractionHintEventBusPresenter : MonoBehaviour
    {
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private ActiveInputType _defaultInputType = ActiveInputType.KeyboardAndMouse;

        [InjectOptional] private IUIService _uiService;

        private EventBus _eventBus;
        private string _currentHint = string.Empty;
        private ActiveInputType _currentInputType;
        private bool _isSubscribed;

        private void OnEnable()
        {
            _currentInputType = _defaultInputType;
            TrySubscribeToEventBus();
        }

        private void Update()
        {
            TrySubscribeToEventBus();
        }

        private void OnDisable()
        {
            UnsubscribeFromEventBus();
        }

        private void OnHintChanged(InteractionHintStateChangedEvent hintChangedEvent)
        {
            _currentHint = hintChangedEvent.IsVisible
                ? hintChangedEvent.HintText ?? string.Empty
                : string.Empty;

            RefreshPanel();
        }

        private void OnActiveInputTypeChanged(ActiveInputTypeChangedEvent inputTypeChangedEvent)
        {
            _currentInputType = inputTypeChangedEvent.InputType;
            RefreshPanel();
        }

        private void RefreshPanel()
        {
            if (_uiService == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_currentHint))
            {
                _uiService.Close<InteractionHintPanel>();
                return;
            }

            var panel = _uiService.Open<InteractionHintPanel>();
            panel?.SetHint(_currentHint, _currentInputType);
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

            _eventBus.Subscribe<InteractionHintStateChangedEvent>(OnHintChanged);
            _eventBus.Subscribe<ActiveInputTypeChangedEvent>(OnActiveInputTypeChanged);
            _isSubscribed = true;
        }

        private void UnsubscribeFromEventBus()
        {
            if (!_isSubscribed || _eventBus == null)
            {
                return;
            }

            _eventBus.Unsubscribe<InteractionHintStateChangedEvent>(OnHintChanged);
            _eventBus.Unsubscribe<ActiveInputTypeChangedEvent>(OnActiveInputTypeChanged);
            _isSubscribed = false;
            _eventBus = null;
        }
    }
}
