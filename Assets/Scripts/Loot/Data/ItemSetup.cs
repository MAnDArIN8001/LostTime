using System;
using UnityEngine;

namespace Loot.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "NewItemSetup", menuName = "Gameplay/Items/Item Setup", order = 0)]
    public class ItemSetup : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField, Space] public Sprite Icon { get; private set; }
        
#if UNITY_EDITOR

        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(ID))
            {
                return;
            }
            
            ID = Guid.NewGuid().ToString();
                
            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif
    }
}