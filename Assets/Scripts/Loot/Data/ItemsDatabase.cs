using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Linq;
using CodeGeneration;
#endif

namespace Loot.Data
{
    [CreateAssetMenu(fileName = "NewItemsDatabase", menuName = "Gameplay/Items/Item Database", order = 0)]
    public class ItemsDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemSetup> _itemsSetups;

        public IReadOnlyList<ItemSetup> ItemsSetups => _itemsSetups;

#if UNITY_EDITOR
        [ContextMenu("Generate Key Class")]
        private void GenerateKeyClass()
        {
            var settings = CodeGenerationSettingsProvider.GetOrDefault();
            var generationTarget = settings.ResolveForSourceTypeName(
                nameof(ItemsDatabase),
                "ItemKeys");
            var constPairs = (_itemsSetups ?? new List<ItemSetup>())
                .Where(item => item != null)
                .Select(item => new ConstPair { Id = item.Id, Name = item.Name })
                .ToList();

            ConstKeysGenerator.GenerateKeysClass(
                generationTarget.ClassName,
                constPairs,
                generationTarget.OutputFolderPath,
                generationTarget.NamespaceName);
        }
#endif
    }
}