using System;

namespace Loot.Inventory
{
    public interface IInventory
    {
        public event Action<string, int> OnInventoryUpdate;
        public event Action<string, int> OnInventoryElementIncreased;
        public event Action<string, int> OnInventoryElementDecreased;

        public void IncreaseElementCount(string id, int count);
        public void DecreaseElementCount(string id, int count);

        public int GetElementCount(string id);
        public void SetElementCount(string id, int count);
    }
}