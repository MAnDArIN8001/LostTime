using Combat;
using Loot.Systems;
using Quest;
using TMPro;
using UnityEngine;

namespace UI
{
    public class VerticalSliceHudPresenter : MonoBehaviour
    {
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

        private void OnEnable()
        {
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
                _interactionController.FocusHintChanged += OnInteractionHintChanged;
                OnInteractionHintChanged(_interactionController.CurrentInteractHint);
            }
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
                _interactionController.FocusHintChanged -= OnInteractionHintChanged;
            }
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
        }

        private void OnInteractionHintChanged(string hint)
        {
            if (_interactionHintLabel != null)
            {
                _interactionHintLabel.text = string.IsNullOrWhiteSpace(hint)
                    ? string.Empty
                    : $"[E] {hint}";
            }
        }
    }
}
