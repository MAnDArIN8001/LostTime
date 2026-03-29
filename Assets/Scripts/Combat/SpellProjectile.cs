using UnityEngine;

namespace Combat
{
    public class SpellProjectile : MonoBehaviour
    {
        private Vector3 _direction;
        private float _speed;
        private float _damage;
        private float _lifetime;
        private float _spawnTime;
        private GameObject _caster;
        private bool _initialized;

        public void Initialize(Vector3 direction, float speed, float damage, float lifetime, GameObject caster)
        {
            _direction = direction.sqrMagnitude > 0.001f ? direction.normalized : transform.forward;
            _speed = Mathf.Max(0f, speed);
            _damage = Mathf.Max(0f, damage);
            _lifetime = Mathf.Max(0.1f, lifetime);
            _spawnTime = Time.time;
            _caster = caster;
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized)
            {
                return;
            }

            transform.position += _direction * (_speed * Time.deltaTime);

            if (Time.time >= _spawnTime + _lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_initialized || other.gameObject == _caster)
            {
                return;
            }

            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.ApplyDamage(_damage, _caster);
            }

            Destroy(gameObject);
        }
    }
}
