using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UI.Runtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Dialogue.UI
{
    public sealed class DialoguePanel : AbstractUIPanel
    {
        [Serializable]
        public sealed class OptionView
        {
            public string OptionId;
            public string Text;
        }

        [SerializeField] private TMP_Text _speakerLabel;
        [SerializeField] private TMP_Text _bodyLabel;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Transform _optionsRoot;
        [SerializeField] private Button _optionButtonTemplate;
        [SerializeField] private Selectable _defaultGamepadSelection;
        [SerializeField] private bool _canCancel = true;
        [SerializeField, Min(0.001f)] private float _typingCharsPerSecond = 40f;

        private readonly List<Button> _spawnedButtons = new();
        private readonly List<string> _optionIdsByIndex = new();
        private IReadOnlyList<OptionView> _pendingOptions;
        private bool _isChoiceNode;
        private bool _isTyping;
        private Tween _typingTween;
        private string _fullText = string.Empty;

        public event Action ContinueRequested;
        public event Action<string> OptionRequested;
        public event Action CancelRequested;

        protected override void OnAfterShow()
        {
            base.OnAfterShow();
            if (_defaultGamepadSelection != null)
            {
                _defaultGamepadSelection.Select();
            }
        }

        private void OnEnable()
        {
            if (_continueButton != null)
            {
                _continueButton.onClick.AddListener(HandleContinueClicked);
            }
        }

        private void OnDisable()
        {
            if (_continueButton != null)
            {
                _continueButton.onClick.RemoveListener(HandleContinueClicked);
            }

            StopTypingTween();
        }

        private void Update()
        {
            if (_canCancel && IsCancelPressed())
            {
                CancelRequested?.Invoke();
            }
        }

        private static bool IsCancelPressed()
        {
            var keyboard = Keyboard.current;
            return keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
        }

        public void Render(DialogueViewModel model)
        {
            var speaker = model?.SpeakerName ?? string.Empty;
            var text = model?.Text ?? string.Empty;
            _isChoiceNode = model != null && model.NodeType == Dialogue.Core.DialogueNodeType.Choice;
            _pendingOptions = model?.Options;

            if (_speakerLabel != null)
            {
                _speakerLabel.text = speaker;
            }

            if (_continueButton != null)
            {
                _continueButton.gameObject.SetActive(!_isChoiceNode);
            }

            StartTyping(text);
        }

        private void RebuildOptions(IReadOnlyList<OptionView> options)
        {
            ClearOptions();
            if (_optionsRoot == null || _optionButtonTemplate == null || options == null)
            {
                return;
            }

            for (var i = 0; i < options.Count; i++)
            {
                var item = options[i];
                if (item == null)
                {
                    continue;
                }

                var button = Instantiate(_optionButtonTemplate, _optionsRoot);
                button.gameObject.SetActive(true);
                var textLabel = button.GetComponentInChildren<TMP_Text>();
                if (textLabel != null)
                {
                    textLabel.text = item.Text ?? string.Empty;
                }

                var optionId = item.OptionId ?? string.Empty;
                button.onClick.AddListener(() => OptionRequested?.Invoke(optionId));
                _spawnedButtons.Add(button);
                _optionIdsByIndex.Add(optionId);
            }

            if (_spawnedButtons.Count > 0)
            {
                _spawnedButtons[0].Select();
            }
            else if (_continueButton != null && _continueButton.gameObject.activeInHierarchy)
            {
                _continueButton.Select();
            }
        }

        private void ClearOptions()
        {
            for (var i = 0; i < _spawnedButtons.Count; i++)
            {
                var button = _spawnedButtons[i];
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }

            _spawnedButtons.Clear();
            _optionIdsByIndex.Clear();
        }

        private void HandleContinueClicked()
        {
            if (_isTyping)
            {
                CompleteTypingImmediately();
                return;
            }

            ContinueRequested?.Invoke();
        }

        private void StartTyping(string text)
        {
            StopTypingTween();
            ClearOptions();

            _fullText = text ?? string.Empty;
            if (_bodyLabel == null)
            {
                OnTypingCompleted();
                return;
            }

            _bodyLabel.text = _fullText;
            _bodyLabel.maxVisibleCharacters = 0;
            _bodyLabel.ForceMeshUpdate();
            var totalChars = _bodyLabel.textInfo.characterCount;

            if (totalChars <= 0)
            {
                OnTypingCompleted();
                return;
            }

            _isTyping = true;
            var duration = Mathf.Max(0.01f, totalChars / Mathf.Max(1f, _typingCharsPerSecond));
            _typingTween = DOTween.To(
                    () => 0,
                    value => _bodyLabel.maxVisibleCharacters = value,
                    totalChars,
                    duration)
                .SetEase(Ease.Linear)
                .OnComplete(OnTypingCompleted);
        }

        private void CompleteTypingImmediately()
        {
            if (_bodyLabel != null)
            {
                _bodyLabel.maxVisibleCharacters = int.MaxValue;
            }

            StopTypingTween();
            OnTypingCompleted();
        }

        private void OnTypingCompleted()
        {
            if (!_isTyping && _isChoiceNode && _spawnedButtons.Count > 0)
            {
                return;
            }

            _isTyping = false;
            if (_isChoiceNode)
            {
                RebuildOptions(_pendingOptions);
            }
        }

        private void StopTypingTween()
        {
            if (_typingTween == null)
            {
                return;
            }

            _typingTween.Kill();
            _typingTween = null;
        }
    }
}
