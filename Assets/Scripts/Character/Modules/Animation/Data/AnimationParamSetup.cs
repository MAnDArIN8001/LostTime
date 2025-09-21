using UnityEngine;

namespace Character.Modules.Animation.Data
{
    [CreateAssetMenu(fileName = "NewAnimationParamSetup", menuName = "Gameplay/Animation/Animation Param Setup", order = 0)]
    public class AnimationParamSetup : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField, Space] public ParamType Type { get; private set; }
    }

    public enum ParamType
    {
        Float,
        Int,
        Bool,
        Trigger
    }
}