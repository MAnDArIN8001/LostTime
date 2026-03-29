using UnityEngine;

namespace Enemy.Data
{
    [CreateAssetMenu(fileName = "MeleeBeastSetup", menuName = "LostTime/Enemy/Melee Beast Setup")]
    public class MeleeBeastSetup : ScriptableObject
    {
        [field: SerializeField, Min(1f)] public float MaxHealth { get; private set; } = 40f;
        [field: SerializeField, Min(0f)] public float MoveSpeed { get; private set; } = 3.5f;
        [field: SerializeField, Min(0.1f)] public float DetectionDistance { get; private set; } = 10f;
        [field: SerializeField, Min(0.1f)] public float AttackDistance { get; private set; } = 1.8f;
        [field: SerializeField, Min(0f)] public float Damage { get; private set; } = 15f;
        [field: SerializeField, Min(0f)] public float AttackCooldown { get; private set; } = 1.2f;
    }
}
