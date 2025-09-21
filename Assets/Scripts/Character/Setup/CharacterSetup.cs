using UnityEngine;

namespace Character.Setup
{
    [CreateAssetMenu(fileName = "NewCharacterSetup", menuName = "Configuration/Character", order = 0)]
    public class CharacterSetup : ScriptableObject
    {
        [field: SerializeField] public float RunSpeed { get; private set; }
        [field: SerializeField] public float WalkSpeed { get; private set; }
    }
}