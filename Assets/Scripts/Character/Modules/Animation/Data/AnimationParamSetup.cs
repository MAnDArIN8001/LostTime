using System;
using UnityEngine;

namespace Character.Modules.Animation.Data
{
    [CreateAssetMenu(fileName = "NewAnimationParamSetup", menuName = "Gameplay/Animation/Animation Param Setup", order = 0)]
    public class AnimationParamSetup : ScriptableObject
    {
        public string Id { get; private set; }
        
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField, Space] public ParamType Type { get; private set; }
        
        #if UNITY_EDITOR

        private void OnValidate()
        {
            if (!string.IsNullOrWhiteSpace(Id))
            {
                return;
            }
            
            Id = Guid.NewGuid().ToString();
            
            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif
    }

    public enum ParamType
    {
        Float,
        Int,
        Bool,
        Trigger
    }
}