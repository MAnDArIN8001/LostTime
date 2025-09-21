using UnityEngine;
using System.Collections.Generic;

namespace Loot.Data
{
    [CreateAssetMenu(fileName = "NewItemsDatabase", menuName = "Gameplay/Items/Item Database", order = 0)]
    public class ItemsDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemSetup> _itemsSetups;

        public IReadOnlyList<ItemSetup> ItemsSetups => _itemsSetups;
    }
}