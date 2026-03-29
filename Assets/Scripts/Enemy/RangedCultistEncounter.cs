using System;
using Combat;
using Enemy.Data;
using UnityEngine;

namespace Enemy
{
    public class RangedCultistEncounter : MonoBehaviour, IDamageable, IEncounterEnemy
    {
        [SerializeField] private RangedCultistSetup _setup;
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _projectileOrigin;

        private float _currentHealth;
        private float _nextAttackAvailableAt;
        private bool _isDead;

        public bool IsDead => _isDead;

        public event Action<IEncounterEnemy> Died;

        private void Awake()
        {
            _currentHealth = _setup != null ? _setup.MaxHealth : 0f;
        }

        private void Update()
        {
            if (_setup == null || _isDead)
            {
                return;
            }

            if (_target == null)
            {
                ResolveTarget();
            }

            if (_target == null)
            {
                return;
            }

            var toTarget = _target.position - transform.position;
            var distance = toTarget.magnitude;
            if (distance > _setup.DetectionDistance)
            {
                return;
            }

            var flatDirection = new Vector3(toTarget.x, 0f, toTarget.z);
            if (flatDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(flatDirection.normalized),
                    12f * Time.deltaTime);
            }

            if (distance > _setup.PreferredRange)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _target.position,
                    _setup.MoveSpeed * Time.deltaTime);
            }
            else if (distance < _setup.MinimumRange)
            {
                var retreatDirection = -flatDirection.normalized;
                transform.position += retreatDirection * (_setup.MoveSpeed * Time.deltaTime);
            }

            if (Time.time < _nextAttackAvailableAt)
            {
                return;
            }

            FireAtTarget(toTarget.normalized);
            _nextAttackAvailableAt = Time.time + _setup.AttackCooldown;
        }

        public void ApplyDamage(float damage, GameObject damageSource)
        {
            if (_setup == null || _isDead || damage <= 0f)
            {
                return;
            }

            _currentHealth = Mathf.Max(0f, _currentHealth - damage);
            if (_currentHealth > 0f)
            {
                return;
            }

            _isDead = true;
            Died?.Invoke(this);
            Destroy(gameObject);
        }

        private void ResolveTarget()
        {
            var player = FindFirstObjectByType<Character.Character>();
            _target = player != null ? player.transform : null;
        }

        private void FireAtTarget(Vector3 direction)
        {
            if (_setup.ProjectilePrefab != null)
            {
                var origin = _projectileOrigin != null ? _projectileOrigin : transform;
                var projectile = Instantiate(_setup.ProjectilePrefab, origin.position, Quaternion.LookRotation(direction));
                projectile.Initialize(
                    direction,
                    _setup.ProjectileSpeed,
                    _setup.ProjectileDamage,
                    _setup.ProjectileLifetime,
                    gameObject);
                return;
            }

            if (_target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.ApplyDamage(_setup.ProjectileDamage, gameObject);
            }
        }
    }
}
