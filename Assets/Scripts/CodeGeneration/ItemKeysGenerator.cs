using System.Collections.Generic;
using System.IO;
using Loot.Data;
using UnityEditor;

namespace CodeGeneration
{
    public class ItemKeysGenerator
    {
        public void GenerateItemKeysClass(List<ItemSetup> itemsSetups)
        {
            string folderPath = "Assets/Generated";
            string filePath = Path.Combine(folderPath, "ItemKeys.cs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("// AUTO-GENERATED CODE. DO NOT EDIT.");
                writer.WriteLine("namespace Loot.Data");
                writer.WriteLine("{");
                writer.WriteLine("    public static class ItemKeys");
                writer.WriteLine("    {");

                foreach (var item in itemsSetups)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    
                    var safeName = MakeSafeName(item.Name);
                    
                    writer.WriteLine($"        public const string {safeName} = \"{item.ID}\";");
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
        
        private string MakeSafeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "UnnamedItem";
            }
            
            var safe = new System.Text.StringBuilder();
            
            foreach (var c in name)
            {
                safe.Append(char.IsLetterOrDigit(c) ? c : '_');
            }

            return safe.ToString();
        }
    }
}