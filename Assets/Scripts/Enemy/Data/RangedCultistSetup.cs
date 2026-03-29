using UnityEngine;

namespace Enemy.Data
{
    [CreateAssetMenu(fileName = "RangedCultistSetup", menuName = "LostTime/Enemy/Ranged Cultist Setup")]
    public class RangedCultistSetup : ScriptableObject
    {
        [field: SerializeField, Min(1f)] public float MaxHealth { get; private set; } = 30f;
        [field: SerializeField, Min(0f)] public float MoveSpeed { get; private set; } = 3f;
        [field: SerializeField, Min(0.1f)] public float DetectionDistance { get; private set; } = 14f;
        [field: SerializeField, Min(0.1f)] public float PreferredRange { get; private set; } = 7f;
        [field: SerializeField, Min(0.1f)] public float MinimumRange { get; private set; } = 4f;
        [field: SerializeField, Min(0f)] public float ProjectileDamage { get; private set; } = 10f;
        [field: SerializeField, Min(0f)] public float ProjectileSpeed { get; private set; } = 15f;
        [field: SerializeField, Min(0.1f)] public float ProjectileLifetime { get; private set; } = 3f;
        [field: SerializeField, Min(0f)] public float AttackCooldown { get; private set; } = 1.5f;
        [field: SerializeField] public Combat.SpellProjectile ProjectilePrefab { get; private set; }
    }
}
