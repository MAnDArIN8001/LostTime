using System;
using Combat;
using Enemy.Data;
using UnityEngine;

namespace Enemy
{
    public class HeavyGolemEncounter : MonoBehaviour, IDamageable, IEncounterEnemy
    {
        [SerializeField] private HeavyGolemSetup _setup;
        [SerializeField] private Transform _target;

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
            var sqrDistanceToTarget = toTarget.sqrMagnitude;
            var detectionSqr = _setup.DetectionDistance * _setup.DetectionDistance;

            if (sqrDistanceToTarget > detectionSqr)
            {
                return;
            }

            var flatDirection = new Vector3(toTarget.x, 0f, toTarget.z);
            if (flatDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(flatDirection.normalized),
                    7f * Time.deltaTime);
            }

            var attackSqr = _setup.AttackDistance * _setup.AttackDistance;
            if (sqrDistanceToTarget > attackSqr)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _target.position,
                    _setup.MoveSpeed * Time.deltaTime);
                return;
            }

            if (Time.time < _nextAttackAvailableAt)
            {
                return;
            }

            if (_target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.ApplyDamage(_setup.Damage, gameObject);
            }

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
    }
}
