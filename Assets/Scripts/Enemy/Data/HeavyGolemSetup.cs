using UnityEngine;

namespace Enemy.Data
{
    [CreateAssetMenu(fileName = "HeavyGolemSetup", menuName = "LostTime/Enemy/Heavy Golem Setup")]
    public class HeavyGolemSetup : ScriptableObject
    {
        [field: SerializeField, Min(1f)] public float MaxHealth { get; private set; } = 90f;
        [field: SerializeField, Min(0f)] public float MoveSpeed { get; private set; } = 2.1f;
        [field: SerializeField, Min(0.1f)] public float DetectionDistance { get; private set; } = 11f;
        [field: SerializeField, Min(0.1f)] public float AttackDistance { get; private set; } = 2.1f;
        [field: SerializeField, Min(0f)] public float Damage { get; private set; } = 24f;
        [field: SerializeField, Min(0f)] public float AttackCooldown { get; private set; } = 1.8f;
    }
}
