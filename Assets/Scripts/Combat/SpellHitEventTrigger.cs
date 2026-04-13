using System;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public sealed class SpellHitEventTrigger : MonoBehaviour
    {
        [Serializable]
        public sealed class SpellProjectileHitUnityEvent : UnityEvent<SpellProjectile>
        {
        }

        [SerializeField] private bool _triggerOnce;
        [SerializeField] private UnityEvent _onSpellHit;
        [SerializeField] private SpellProjectileHitUnityEvent _onSpellProjectileHit;

        private bool _isTriggered;

        public event Action<SpellProjectile> SpellHit;

        private void OnTriggerEnter(Collider other)
        {
            TryHandleHit(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision == null)
            {
                return;
            }

            TryHandleHit(collision.collider);
        }

        private void TryHandleHit(Collider other)
        {
            if (_triggerOnce && _isTriggered)
            {
                return;
            }

            if (other == null)
            {
                return;
            }

            var projectile = other.GetComponentInParent<SpellProjectile>();
            if (projectile == null)
            {
                return;
            }

            _isTriggered = true;
            _onSpellHit?.Invoke();
            _onSpellProjectileHit?.Invoke(projectile);
            SpellHit?.Invoke(projectile);
        }
    }
}
