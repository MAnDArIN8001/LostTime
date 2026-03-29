using System;
using Combat.Data;
using UnityEngine;

namespace Combat
{
    public class CharacterSpellCaster : MonoBehaviour
    {
        [SerializeField] private ProjectileSpellSetup _spellSetup;
        [SerializeField] private CharacterMana _mana;
        [SerializeField] private Transform _castOrigin;

        private float _nextCastAvailableAt;

        public bool CanCast =>
            _spellSetup != null &&
            _spellSetup.ProjectilePrefab != null &&
            _mana != null &&
            Time.time >= _nextCastAvailableAt &&
            _mana.HasEnough(_spellSetup.ManaCost);

        public event Action<SpellCastInfo> SpellCast;
        public event Action<ManaSpentInfo> ManaSpent;

        public bool TryCast()
        {
            if (!CanCast)
            {
                return false;
            }

            if (!_mana.TrySpend(_spellSetup.ManaCost))
            {
                return false;
            }

            var spawnOrigin = _castOrigin != null ? _castOrigin : transform;
            var projectile = Instantiate(_spellSetup.ProjectilePrefab, spawnOrigin.position, spawnOrigin.rotation);
            projectile.Initialize(
                spawnOrigin.forward,
                _spellSetup.ProjectileSpeed,
                _spellSetup.Damage,
                _spellSetup.ProjectileLifetime,
                gameObject);

            _nextCastAvailableAt = Time.time + _spellSetup.Cooldown;

            ManaSpent?.Invoke(new ManaSpentInfo(_spellSetup.ManaCost, _mana.CurrentMana, _mana.MaxMana));
            SpellCast?.Invoke(new SpellCastInfo(_spellSetup, projectile));
            return true;
        }
    }

    public readonly struct SpellCastInfo
    {
        public SpellCastInfo(ProjectileSpellSetup setup, SpellProjectile projectile)
        {
            Setup = setup;
            Projectile = projectile;
        }

        public ProjectileSpellSetup Setup { get; }
        public SpellProjectile Projectile { get; }
    }

    public readonly struct ManaSpentInfo
    {
        public ManaSpentInfo(float spentAmount, float currentMana, float maxMana)
        {
            SpentAmount = spentAmount;
            CurrentMana = currentMana;
            MaxMana = maxMana;
        }

        public float SpentAmount { get; }
        public float CurrentMana { get; }
        public float MaxMana { get; }
    }
}
