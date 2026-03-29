using System;
using Combat;
using UnityEngine;

namespace Enemy.Data
{
    [Serializable]
    public class GuardianPatternTiming
    {
        [field: SerializeField, Min(0f)] public float TelegraphDuration { get; private set; } = 1f;
        [field: SerializeField, Min(0.1f)] public float ActiveDuration { get; private set; } = 3f;
        [field: SerializeField, Min(0f)] public float CooldownAfter { get; private set; } = 2f;
    }

    [Serializable]
    public class GuardianVolleyPatternData
    {
        [field: SerializeField] public GuardianPatternTiming Timing { get; private set; } = new GuardianPatternTiming();
        [field: SerializeField, Min(1)] public int ShotCount { get; private set; } = 4;
        [field: SerializeField, Min(0f)] public float ShotInterval { get; private set; } = 0.35f;
        [field: SerializeField, Min(0f)] public float SpreadHalfAngleDegrees { get; private set; } = 10f;
        [field: SerializeField] public SpellProjectile ProjectilePrefab { get; private set; }
        [field: SerializeField, Min(0f)] public float ProjectileSpeed { get; private set; } = 14f;
        [field: SerializeField, Min(0f)] public float ProjectileDamage { get; private set; } = 12f;
        [field: SerializeField, Min(0.1f)] public float ProjectileLifetime { get; private set; } = 4f;
    }

    [Serializable]
    public class GuardianZonePatternData
    {
        [field: SerializeField] public GuardianPatternTiming Timing { get; private set; } = new GuardianPatternTiming();
        [field: SerializeField, Min(0.1f)] public float ZoneRadius { get; private set; } = 3.5f;
        [field: SerializeField, Min(0f)] public float DamagePerTick { get; private set; } = 10f;
        [field: SerializeField, Min(0.05f)] public float TickInterval { get; private set; } = 0.5f;
        [field: SerializeField] public LayerMask DamageableLayers { get; private set; }
    }

    [CreateAssetMenu(fileName = "GuardianBossSetup", menuName = "LostTime/Enemy/Guardian Boss Setup")]
    public class GuardianBossSetup : ScriptableObject
    {
        [field: SerializeField, Min(1f)] public float MaxHealth { get; private set; } = 200f;
        [field: SerializeField] public GuardianVolleyPatternData Volley { get; private set; } = new GuardianVolleyPatternData();
        [field: SerializeField] public GuardianZonePatternData Zone { get; private set; } = new GuardianZonePatternData();
    }
}
