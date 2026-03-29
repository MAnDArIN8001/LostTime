using UnityEngine;

namespace Combat.Data
{
    [CreateAssetMenu(fileName = "ProjectileSpellSetup", menuName = "LostTime/Combat/Projectile Spell Setup")]
    public class ProjectileSpellSetup : ScriptableObject
    {
        [field: SerializeField] public SpellProjectile ProjectilePrefab { get; private set; }
        [field: SerializeField, Min(0f)] public float ProjectileSpeed { get; private set; } = 16f;
        [field: SerializeField, Min(0.1f)] public float ProjectileLifetime { get; private set; } = 4f;
        [field: SerializeField, Min(0f)] public float Damage { get; private set; } = 20f;
        [field: SerializeField, Min(0f)] public float ManaCost { get; private set; } = 15f;
        [field: SerializeField, Min(0f)] public float Cooldown { get; private set; } = 1f;
    }
}
