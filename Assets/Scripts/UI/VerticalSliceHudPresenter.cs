using Combat;
using Loot.Items;
using Loot.Systems;
using Quest;
using TMPro;
using UnityEngine;

namespace UI
{
    public class VerticalSliceHudPresenter : MonoBehaviour
    {
        private const string RuntimeLabelPrefix = "[Auto]";

        [Header("Data Sources")]
        [SerializeField] private CharacterVitals _characterVitals;
        [SerializeField] private CharacterMana _characterMana;
        [SerializeField] private VerticalSliceQuestProgression _questProgression;
        [SerializeField] private InteractionController _interactionController;

        [Header("HUD Labels")]
        [SerializeField] private TextMeshProUGUI _healthLabel;
        [SerializeField] private TextMeshProUGUI _manaLabel;
        [SerializeField] private TextMeshProUGUI _objectiveLabel;
        [SerializeField] private TextMeshProUGUI _interactionHintLabel;

        [Header("Hint Presentation")]
        [SerializeField] private string _interactionPrefix = "[Interact]";
        [SerializeField] private bool _useEventBusInteractionHint = true;

        [Header("Feedback")]
        [SerializeField] private AudioSource _feedbackAudioSource;
        [SerializeField] private AudioClip _objectiveChangedClip;
        [SerializeField] private AudioClip _pickupCollectedClip;
        [SerializeField] private AudioClip _interactHintAppearedClip;

        private string _lastHint = string.Empty;

        private void OnEnable()
        {
            EnsureHudLabelRefs();

            if (_characterVitals != null)
            {
                _characterVitals.HealthChanged += OnHealthChanged;
                OnHealthChanged(_characterVitals.CurrentHealth, _characterVitals.MaxHealth);
            }

            if (_characterMana != null)
            {
                _characterMana.ManaChanged += OnManaChanged;
                OnManaChanged(_characterMana.CurrentMana, _characterMana.MaxMana);
            }

            if (_questProgression != null)
            {
                _questProgression.ObjectiveChanged += OnObjectiveChanged;
                OnObjectiveChanged(_questProgression.CurrentObjectiveText);
            }

            if (_interactionController != null)
            {
                _interactionController.PickupCollected += OnPickupCollected;
                if (!_useEventBusInteractionHint)
                {
                    _interactionController.FocusHintChanged += OnInteractionHintChanged;
                    OnInteractionHintChanged(_interactionController.CurrentInteractHint);
                }
            }

            ConsumablePickupItem.Collected += OnConsumableCollected;
        }

        private void OnDisable()
        {
            if (_characterVitals != null)
            {
                _characterVitals.HealthChanged -= OnHealthChanged;
            }

            if (_characterMana != null)
            {
                _characterMana.ManaChanged -= OnManaChanged;
            }

            if (_questProgression != null)
            {
                _questProgression.ObjectiveChanged -= OnObjectiveChanged;
            }

            if (_interactionController != null)
            {
                if (!_useEventBusInteractionHint)
                {
                    _interactionController.FocusHintChanged -= OnInteractionHintChanged;
                }
                _interactionController.PickupCollected -= OnPickupCollected;
            }

            ConsumablePickupItem.Collected -= OnConsumableCollected;
        }

        private void OnHealthChanged(float current, float max)
        {
            if (_healthLabel != null)
            {
                _healthLabel.text = $"HP {Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            }
        }

        private void OnManaChanged(float current, float max)
        {
            if (_manaLabel != null)
            {
                _manaLabel.text = $"MP {Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            }
        }

        private void OnObjectiveChanged(string objective)
        {
            if (_objectiveLabel != null)
            {
                _objectiveLabel.text = string.IsNullOrWhiteSpace(objective)
                    ? string.Empty
                    : $"Objective: {objective}";
            }

            PlayFeedback(_objectiveChangedClip);
        }

        private void OnInteractionHintChanged(string hint)
        {
            if (_interactionHintLabel != null)
            {
                _interactionHintLabel.text = string.IsNullOrWhiteSpace(hint)
                    ? string.Empty
                    : $"{_interactionPrefix} {hint}";
            }

            if (!string.IsNullOrWhiteSpace(hint) && string.IsNullOrWhiteSpace(_lastHint))
            {
                PlayFeedback(_interactHintAppearedClip);
            }

            _lastHint = hint ?? string.Empty;
        }

        private void OnPickupCollected(ITakable takable, GameObject interactor)
        {
            if (takable is Component component && component.GetComponent<ConsumablePickupItem>() != null)
            {
                return;
            }

            PlayFeedback(_pickupCollectedClip);
        }

        private void OnConsumableCollected(ConsumablePickupItem pickupItem, GameObject interactor)
        {
            PlayFeedback(_pickupCollectedClip);
        }

        private void EnsureHudLabelRefs()
        {
            if (_healthLabel == null)
            {
                return;
            }

            var labelsNeedSplit =
                _manaLabel == null ||
                _objectiveLabel == null ||
                _interactionHintLabel == null ||
                ReferenceEquals(_healthLabel, _manaLabel) ||
                ReferenceEquals(_healthLabel, _objectiveLabel) ||
                ReferenceEquals(_healthLabel, _interactionHintLabel) ||
                ReferenceEquals(_manaLabel, _objectiveLabel) ||
                ReferenceEquals(_manaLabel, _interactionHintLabel) ||
                ReferenceEquals(_objectiveLabel, _interactionHintLabel);

            if (!labelsNeedSplit)
            {
                return;
            }

            var root = _healthLabel.rectTransform.parent;
            if (root == null)
            {
                return;
            }

            _healthLabel.name = $"{RuntimeLabelPrefix} HP";
            _manaLabel = CreateLabelClone(_healthLabel, $"{RuntimeLabelPrefix} MP", new Vector2(0f, -42f));
            _objectiveLabel = CreateLabelClone(_healthLabel, $"{RuntimeLabelPrefix} Objective", new Vector2(0f, -84f));
            _interactionHintLabel = CreateLabelClone(_healthLabel, $"{RuntimeLabelPrefix} Interact", new Vector2(0f, -126f));
        }

        private static TextMeshProUGUI CreateLabelClone(
            TextMeshProUGUI source,
            string objectName,
            Vector2 offset)
        {
            var clone = Instantiate(source, source.rectTransform.parent);
            clone.name = objectName;
            clone.text = string.Empty;
            clone.raycastTarget = false;
            clone.rectTransform.anchoredPosition = source.rectTransform.anchoredPosition + offset;
            return clone;
        }

        private void PlayFeedback(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            if (_feedbackAudioSource != null)
            {
                _feedbackAudioSource.PlayOneShot(clip);
                return;
            }

            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }
}
