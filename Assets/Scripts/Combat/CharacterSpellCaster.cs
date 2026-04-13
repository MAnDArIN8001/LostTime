using System;
using Character.Modules.Movement;
using Combat.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Combat
{
    public class CharacterSpellCaster : MonoBehaviour
    {
        [Header("Spell Data")]
        [SerializeField] private ProjectileSpellSetup _spellSetup;
        [SerializeField] private CharacterMana _mana;
        [SerializeField] private Transform _castOrigin;
        [FormerlySerializedAs("_forwardOverride")]
        [SerializeField] private Transform _aim;

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
            var forwardTransform = ResolveProjectileForwardTransform();
            var shootDirection = forwardTransform.forward;
            var shootRotation = Quaternion.LookRotation(shootDirection, Vector3.up);

            var projectile = Instantiate(_spellSetup.ProjectilePrefab, spawnOrigin.position, shootRotation);
            projectile.Initialize(
                shootDirection,
                _spellSetup.ProjectileSpeed,
                _spellSetup.Damage,
                _spellSetup.ProjectileLifetime,
                gameObject);

            _nextCastAvailableAt = Time.time + _spellSetup.Cooldown;
            PlayCastFeedback(spawnOrigin.position, shootRotation);

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

        private Transform ResolveProjectileForwardTransform()
        {
            if (_aim != null)
            {
                return _aim;
            }

            var movement = GetComponentInParent<MovementModule>();
            if (movement != null)
            {
                return movement.Root;
            }

            return transform;
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
