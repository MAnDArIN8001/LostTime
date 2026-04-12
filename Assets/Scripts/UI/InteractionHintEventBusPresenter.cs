using System;
using System.Collections.Generic;
using Gameplay.Input;
using Gameplay.Interaction.Core;
using TMPro;
using UnityEngine;
using Utils.Events;

namespace UI
{
    [DisallowMultipleComponent]
    public sealed class InteractionHintEventBusPresenter : MonoBehaviour
    {
        [Serializable]
        private struct InputButtonBinding
        {
            [SerializeField] private ActiveInputType _inputType;
            [SerializeField] private string _buttonLabel;

            public ActiveInputType InputType => _inputType;
            public string ButtonLabel => _buttonLabel;
        }

        [Header("References")]
        [SerializeField] private SceneEventBusProvider _eventBusProvider;
        [SerializeField] private TMP_Text _hintLabel;

        [Header("Text Format")]
        [SerializeField] private string _hintTextWithButtonFormat = "[{0}] {1}";
        [SerializeField] private string _hintTextWithoutButtonFormat = "{0}";
        [SerializeField] private bool _toggleLabelGameObjectVisibility;

        [Header("Input Buttons")]
        [SerializeField] private ActiveInputType _defaultInputType = ActiveInputType.KeyboardAndMouse;
        [SerializeField] private List<InputButtonBinding> _inputButtons = new();

        private readonly Dictionary<ActiveInputType, string> _buttonByInputType = new();
        private EventBus _eventBus;
        private string _currentHint = string.Empty;
        private ActiveInputType _currentInputType;
        private bool _isSubscribed;

        private void OnEnable()
        {
            _currentInputType = _defaultInputType;
            RebuildBindings();
            SetHintVisible(false);

            TrySubscribeToEventBus();
        }

        private void Update()
        {
            TrySubscribeToEventBus();
        }

        private void OnDisable()
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

        private void OnHintChanged(InteractionHintStateChangedEvent hintChangedEvent)
        {
            _currentHint = hintChangedEvent.IsVisible
                ? hintChangedEvent.HintText ?? string.Empty
                : string.Empty;

            RefreshView();
        }

        private void OnActiveInputTypeChanged(ActiveInputTypeChangedEvent inputTypeChangedEvent)
        {
            _currentInputType = inputTypeChangedEvent.InputType;
            RefreshView();
        }

        private void RebuildBindings()
        {
            _buttonByInputType.Clear();

            for (var i = 0; i < _inputButtons.Count; i++)
            {
                var binding = _inputButtons[i];
                _buttonByInputType[binding.InputType] = binding.ButtonLabel;
            }
        }

        private void RefreshView()
        {
            if (_hintLabel == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_currentHint))
            {
                SetHintVisible(false);
                _hintLabel.text = string.Empty;
                return;
            }

            SetHintVisible(true);

            if (!_buttonByInputType.TryGetValue(_currentInputType, out var buttonLabel) ||
                string.IsNullOrWhiteSpace(buttonLabel))
            {
                _hintLabel.text = string.Format(_hintTextWithoutButtonFormat, _currentHint);
                return;
            }

            _hintLabel.text = string.Format(_hintTextWithButtonFormat, buttonLabel, _currentHint);
        }

        private void SetHintVisible(bool isVisible)
        {
            if (_hintLabel != null && _toggleLabelGameObjectVisibility)
            {
                _hintLabel.gameObject.SetActive(isVisible);
            }
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
    }
}
