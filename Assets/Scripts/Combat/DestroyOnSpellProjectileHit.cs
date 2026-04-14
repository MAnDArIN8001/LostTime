using UnityEngine;

namespace Combat
{
    [DisallowMultipleComponent]
    public sealed class DestroyOnSpellProjectileHit : MonoBehaviour
    {
        [SerializeField] private SpellHitEventTrigger _spellHitTrigger;
        [SerializeField] private GameObject _targetToDestroy;
        [SerializeField] private GameObject _destroyParticlesPrefab;
        [SerializeField] private Transform _particlesSpawnPoint;

        private bool _isDestroyed;

        private void Reset()
        {
            if (_spellHitTrigger == null)
            {
                _spellHitTrigger = GetComponent<SpellHitEventTrigger>();
            }
        }

        private void OnEnable()
        {
            if (_spellHitTrigger != null)
            {
                _spellHitTrigger.SpellHit += OnSpellHit;
            }
        }

        private void OnDisable()
        {
            if (_spellHitTrigger != null)
            {
                _spellHitTrigger.SpellHit -= OnSpellHit;
            }
        }

        private void OnSpellHit(SpellProjectile projectile)
        {
            if (_isDestroyed || projectile == null)
            {
                return;
            }

            _isDestroyed = true;
            SpawnParticles();

            var target = _targetToDestroy != null ? _targetToDestroy : gameObject;
            if (target != null)
            {
                Destroy(target);
            }
        }

        private void SpawnParticles()
        {
            if (_destroyParticlesPrefab == null)
            {
                return;
            }

            var spawnPoint = _particlesSpawnPoint != null ? _particlesSpawnPoint : transform;
            Instantiate(_destroyParticlesPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
