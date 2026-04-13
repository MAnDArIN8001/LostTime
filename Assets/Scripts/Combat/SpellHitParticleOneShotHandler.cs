using UnityEngine;

namespace Combat
{
    public sealed class SpellHitParticleOneShotHandler : MonoBehaviour
    {
        [SerializeField] private SpellHitEventTrigger _spellHitTrigger;
        [SerializeField] private ParticleSystem[] _particleSystems = System.Array.Empty<ParticleSystem>();
        [SerializeField] private bool _playOnlyOnce = true;
        [SerializeField] private bool _reactivateParticleGameObjects = true;

        private bool _played;

        private void Reset()
        {
            if (_spellHitTrigger == null)
            {
                _spellHitTrigger = GetComponent<SpellHitEventTrigger>();
            }

            if (_particleSystems == null || _particleSystems.Length == 0)
            {
                _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
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

        public void HandleSpellHit()
        {
            TryPlayParticles();
        }

        private void OnSpellHit(SpellProjectile projectile)
        {
            TryPlayParticles();
        }

        private void TryPlayParticles()
        {
            if (_playOnlyOnce && _played)
            {
                return;
            }

            var playedAny = false;
            for (var i = 0; i < _particleSystems.Length; i++)
            {
                var ps = _particleSystems[i];
                if (ps == null)
                {
                    continue;
                }

                if (_reactivateParticleGameObjects && !ps.gameObject.activeSelf)
                {
                    ps.gameObject.SetActive(true);
                }

                ps.Play(true);
                playedAny = true;
            }

            if (playedAny)
            {
                _played = true;
            }
        }

        public void ResetOneShot()
        {
            _played = false;
        }
    }
}
