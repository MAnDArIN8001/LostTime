using System;
using Combat;
using Enemy.Data;
using UnityEngine;

namespace Enemy
{
    public enum GuardianCyclePhase
    {
        VolleyTelegraph = 0,
        VolleyActive = 1,
        VolleyCooldown = 2,
        ZoneTelegraph = 3,
        ZoneActive = 4,
        ZoneCooldown = 5,
    }

    public class TrialGuardianEncounter : MonoBehaviour, IDamageable, IEncounterEnemy
    {
        [SerializeField] private GuardianBossSetup _setup;
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _projectileOrigin;

        [Header("Feedback")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _volleyTelegraphClip;
        [SerializeField] private AudioClip _zoneTelegraphClip;
        [SerializeField] private AudioClip _phaseSwitchClip;
        [SerializeField] private GameObject _volleyTelegraphVfxPrefab;
        [SerializeField] private GameObject _zoneTelegraphVfxPrefab;

        private float _currentHealth;
        private bool _isDead;
        private GuardianCyclePhase _phase;
        private float _phaseEnteredAt;
        private int _volleyShotsFired;
        private float _nextVolleyShotAt;
        private float _nextZoneTickAt;

        public bool IsDead => _isDead;

        public event Action<IEncounterEnemy> Died;
        public event Action<GuardianCyclePhase> PhaseChanged;

        private void Awake()
        {
            _currentHealth = _setup != null ? _setup.MaxHealth : 0f;
            EnterPhase(GuardianCyclePhase.VolleyTelegraph);
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

            FaceTarget();

            var elapsedInPhase = Time.time - _phaseEnteredAt;

            switch (_phase)
            {
                case GuardianCyclePhase.VolleyTelegraph:
                    if (elapsedInPhase >= _setup.Volley.Timing.TelegraphDuration)
                    {
                        EnterPhase(GuardianCyclePhase.VolleyActive);
                    }
                    break;

                case GuardianCyclePhase.VolleyActive:
                    TickVolleyActive();
                    if (elapsedInPhase >= _setup.Volley.Timing.ActiveDuration)
                    {
                        EnterPhase(GuardianCyclePhase.VolleyCooldown);
                    }
                    break;

                case GuardianCyclePhase.VolleyCooldown:
                    if (elapsedInPhase >= _setup.Volley.Timing.CooldownAfter)
                    {
                        EnterPhase(GuardianCyclePhase.ZoneTelegraph);
                    }
                    break;

                case GuardianCyclePhase.ZoneTelegraph:
                    if (elapsedInPhase >= _setup.Zone.Timing.TelegraphDuration)
                    {
                        EnterPhase(GuardianCyclePhase.ZoneActive);
                    }
                    break;

                case GuardianCyclePhase.ZoneActive:
                    TickZoneActive();
                    if (elapsedInPhase >= _setup.Zone.Timing.ActiveDuration)
                    {
                        EnterPhase(GuardianCyclePhase.ZoneCooldown);
                    }
                    break;

                case GuardianCyclePhase.ZoneCooldown:
                    if (elapsedInPhase >= _setup.Zone.Timing.CooldownAfter)
                    {
                        EnterPhase(GuardianCyclePhase.VolleyTelegraph);
                    }
                    break;
            }
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

        private void EnterPhase(GuardianCyclePhase phase)
        {
            _phase = phase;
            _phaseEnteredAt = Time.time;
            PhaseChanged?.Invoke(phase);
            PlayPhaseFeedback(phase);

            switch (phase)
            {
                case GuardianCyclePhase.VolleyActive:
                    _volleyShotsFired = 0;
                    _nextVolleyShotAt = Time.time;
                    break;
                case GuardianCyclePhase.ZoneActive:
                    _nextZoneTickAt = Time.time;
                    break;
            }
        }

        private void FaceTarget()
        {
            var toTarget = _target.position - transform.position;
            var flat = new Vector3(toTarget.x, 0f, toTarget.z);
            if (flat.sqrMagnitude <= 0.001f)
            {
                return;
            }

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(flat.normalized),
                10f * Time.deltaTime);
        }

        private void TickVolleyActive()
        {
            var v = _setup.Volley;
            var phaseEnd = _phaseEnteredAt + v.Timing.ActiveDuration;

            while (_volleyShotsFired < v.ShotCount && Time.time < phaseEnd && Time.time >= _nextVolleyShotAt)
            {
                FireVolleyShot(v);
                _volleyShotsFired++;
                _nextVolleyShotAt = Time.time + v.ShotInterval;
            }
        }

        private void FireVolleyShot(GuardianVolleyPatternData v)
        {
            if (v.ProjectilePrefab == null)
            {
                if (_target.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.ApplyDamage(v.ProjectileDamage, gameObject);
                }
                return;
            }

            var origin = _projectileOrigin != null ? _projectileOrigin : transform;
            var toTarget = _target.position - origin.position;
            var baseDir = toTarget.sqrMagnitude > 0.001f ? toTarget.normalized : transform.forward;

            var spread = v.SpreadHalfAngleDegrees;
            var yaw = spread > 0f ? UnityEngine.Random.Range(-spread, spread) : 0f;
            var direction = Quaternion.AngleAxis(yaw, Vector3.up) * baseDir;

            var projectile = Instantiate(v.ProjectilePrefab, origin.position, Quaternion.LookRotation(direction));
            projectile.Initialize(
                direction,
                v.ProjectileSpeed,
                v.ProjectileDamage,
                v.ProjectileLifetime,
                gameObject);
        }

        private void TickZoneActive()
        {
            var z = _setup.Zone;
            var phaseEnd = _phaseEnteredAt + z.Timing.ActiveDuration;

            while (Time.time <= phaseEnd && Time.time >= _nextZoneTickAt)
            {
                ApplyZoneTick(z);
                _nextZoneTickAt = Time.time + z.TickInterval;
            }
        }

        private void ApplyZoneTick(GuardianZonePatternData z)
        {
            var center = _target.position;
            var mask = z.DamageableLayers.value == 0 ? Physics.AllLayers : z.DamageableLayers.value;
            var hits = Physics.OverlapSphere(center, z.ZoneRadius, mask, QueryTriggerInteraction.Collide);
            for (var i = 0; i < hits.Length; i++)
            {
                var col = hits[i];
                if (col == null || col.gameObject == gameObject)
                {
                    continue;
                }

                if (col.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.ApplyDamage(z.DamagePerTick, gameObject);
                }
            }
        }

        private void ResolveTarget()
        {
            var player = FindFirstObjectByType<Character.Character>();
            _target = player != null ? player.transform : null;
        }

        private void PlayPhaseFeedback(GuardianCyclePhase phase)
        {
            switch (phase)
            {
                case GuardianCyclePhase.VolleyTelegraph:
                    SpawnTelegraphVfx(_volleyTelegraphVfxPrefab);
                    PlayAudio(_volleyTelegraphClip);
                    break;
                case GuardianCyclePhase.ZoneTelegraph:
                    SpawnTelegraphVfx(_zoneTelegraphVfxPrefab);
                    PlayAudio(_zoneTelegraphClip);
                    break;
                case GuardianCyclePhase.VolleyActive:
                case GuardianCyclePhase.ZoneActive:
                    PlayAudio(_phaseSwitchClip);
                    break;
            }
        }

        private void SpawnTelegraphVfx(GameObject prefab)
        {
            if (prefab == null)
            {
                return;
            }

            var spawnPosition = _target != null ? _target.position : transform.position;
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }

        private void PlayAudio(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(clip);
                return;
            }

            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_setup == null || _target == null)
            {
                return;
            }

            Gizmos.color = new Color(1f, 0.4f, 0.1f, 0.35f);
            Gizmos.DrawWireSphere(_target.position, _setup.Zone.ZoneRadius);
        }
#endif
    }
}
