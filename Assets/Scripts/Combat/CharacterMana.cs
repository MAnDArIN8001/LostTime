using System;
using UnityEngine;

namespace Combat
{
    public class CharacterMana : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float _maxMana = 100f;
        [SerializeField, Min(0f)] private float _manaRegenPerSecond = 5f;

        private float _currentMana;

        public float MaxMana => _maxMana;
        public float CurrentMana => _currentMana;

        public event Action<float, float> ManaChanged;

        private void Awake()
        {
            _currentMana = _maxMana;
            ManaChanged?.Invoke(_currentMana, _maxMana);
        }

        private void Update()
        {
            if (_manaRegenPerSecond <= 0f || _currentMana >= _maxMana)
            {
                return;
            }

            _currentMana = Mathf.Min(_maxMana, _currentMana + (_manaRegenPerSecond * Time.deltaTime));
            ManaChanged?.Invoke(_currentMana, _maxMana);
        }

        public bool HasEnough(float requiredMana) => _currentMana >= requiredMana;

        public bool TrySpend(float manaCost)
        {
            if (manaCost < 0f || !HasEnough(manaCost))
            {
                return false;
            }

            _currentMana -= manaCost;
            ManaChanged?.Invoke(_currentMana, _maxMana);
            return true;
        }

        public void Restore(float amount)
        {
            if (amount <= 0f || _currentMana >= _maxMana)
            {
                return;
            }

            _currentMana = Mathf.Min(_maxMana, _currentMana + amount);
            ManaChanged?.Invoke(_currentMana, _maxMana);
        }
    }
}
