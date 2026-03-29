using System;
using Loot.Data;
using Loot.Systems;
using UnityEngine;

namespace Loot.Items
{
    public class LootableItem : MonoBehaviour, ITakable
    {
        public event Action<ITakable> OnItemTaken;
        
        [field: SerializeField] public ItemSetup ItemSetup { get; private set; }
        [field: SerializeField] public string InteractionPrompt { get; private set; } = "Take";
        
        public void Take()
        {
            Destroy(gameObject);
            
            OnItemTaken?.Invoke(this);
        }
    }
}