using System;
using Combat.Data;
using UnityEngine;

namespace Combat
{
    public class CharacterSpellCaster : MonoBehaviour
    {
        [Header("Spell Data")]
        [SerializeField] private ProjectileSpellSetup _spellSetup;
        [SerializeField] private CharacterMana _mana;
        [SerializeField] private Transform _castOrigin;

        [Header("Feedback")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _castClip;
        [SerializeField] private GameObject _castVfxPrefab;

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
            PlayCastFeedback(spawnOrigin.position, spawnOrigin.rotation);

            ManaSpent?.Invoke(new ManaSpentInfo(_spellSetup.ManaCost, _mana.CurrentMana, _mana.MaxMana));
            SpellCast?.Invoke(new SpellCastInfo(_spellSetup, projectile));
            return true;
        }

        private void PlayCastFeedback(Vector3 position, Quaternion rotation)
        {
            if (_castVfxPrefab != null)
            {
                Instantiate(_castVfxPrefab, position, rotation);
            }

            if (_castClip == null)
            {
                return;
            }

            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(_castClip);
                return;
            }

            AudioSource.PlayClipAtPoint(_castClip, position);
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
