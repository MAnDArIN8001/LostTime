using System;
using Loot.Data;
using UnityEngine;

namespace Loot.Systems
{
    public class LootItem : MonoBehaviour, ITakable
    {
        public event Action<ITakable> OnItemTaken;

        [SerializeField] private ItemSetup _itemSetup;
        [SerializeField] private string _interactionPrompt = "Take";

        public ItemSetup ItemSetup => _itemSetup;
        public string InteractionPrompt => _interactionPrompt;
        
        public void Take()
        {
            OnItemTaken?.Invoke(this);
            
            Destroy(gameObject);
        }
    }
}