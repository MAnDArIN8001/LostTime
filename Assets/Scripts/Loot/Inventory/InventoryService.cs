using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loot.Inventory
{
    public class InventoryService : IInventory, IDisposable
    {
        public event Action<string, int> OnInventoryUpdate;
        public event Action<string, int> OnInventoryElementIncreased;
        public event Action<string, int> OnInventoryElementDecreased;
        
        private readonly Dictionary<string, int> _inventory = new();

        public void IncreaseElementCount(string id, int count)
        {
            if (_inventory.TryGetValue(id, out var value))
            {
                _inventory[id] = value + count;
                
                OnInventoryUpdate?.Invoke(id, _inventory[id]);
                OnInventoryElementIncreased?.Invoke(id, count);

                return;
            }
            
            _inventory.Add(id, count);
            
            OnInventoryUpdate?.Invoke(id, _inventory[id]);
            OnInventoryElementIncreased?.Invoke(id, count);
        }

        public void DecreaseElementCount(string id, int count)
        {
            if (!_inventory.TryGetValue(id, out var value))
            {
                Debug.LogWarning($"Inventory doesn't contains item with id: {id}");
                
                return;
            }

            if (value - count < 0)
            {
                Debug.LogWarning($"Can't decrease more, than have in inventory: {id}");
                
                return;
            }
            
            _inventory[id] = value - count;
                
            OnInventoryUpdate?.Invoke(id, _inventory[id]);
            OnInventoryElementDecreased?.Invoke(id, count);
        }

        public int GetElementCount(string id)
        {
            _inventory.TryGetValue(id, out var value);

            return value;
        }

        public void SetElementCount(string id, int count)
        {
            if (_inventory.TryGetValue(id, out var value))
            {
                _inventory[id] = count;
                
                OnInventoryUpdate?.Invoke(id, _inventory[id]);

                return;
            }
            
            _inventory.Add(id, count);
            
            OnInventoryUpdate?.Invoke(id, _inventory[id]);
        }
            
        public void Dispose()
        {
            _inventory.Clear();
        }
    }
}