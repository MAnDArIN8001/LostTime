using System;
using System.Collections.Generic;
using Gameplay.Input;
using TMPro;
using UI.Runtime;
using UnityEngine;

namespace UI
{
    public sealed class InteractionHintPanel : AbstractUIPanel
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
        [SerializeField] private TMP_Text _hintLabel;

        [Header("Text Format")]
        [SerializeField] private string _hintTextWithButtonFormat = "[{0}] {1}";
        [SerializeField] private string _hintTextWithoutButtonFormat = "{0}";
        [SerializeField] private bool _toggleLabelGameObjectVisibility;

        [Header("Input Buttons")]
        [SerializeField] private List<InputButtonBinding> _inputButtons = new();

        private readonly Dictionary<ActiveInputType, string> _buttonByInputType = new();

        protected override void Awake()
        {
            base.Awake();
            RebuildBindings();
        }

        public void SetHint(string hintText, ActiveInputType inputType)
        {
            if (_hintLabel == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(hintText))
            {
                _hintLabel.text = string.Empty;
                SetHintVisible(false);
                return;
            }

            SetHintVisible(true);
            if (!_buttonByInputType.TryGetValue(inputType, out var buttonLabel) || string.IsNullOrWhiteSpace(buttonLabel))
            {
                _hintLabel.text = string.Format(_hintTextWithoutButtonFormat, hintText);
                return;
            }

            _hintLabel.text = string.Format(_hintTextWithButtonFormat, buttonLabel, hintText);
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

        private void SetHintVisible(bool isVisible)
        {
            if (_hintLabel != null && _toggleLabelGameObjectVisibility)
            {
                _hintLabel.gameObject.SetActive(isVisible);
            }
        }
    }
}
