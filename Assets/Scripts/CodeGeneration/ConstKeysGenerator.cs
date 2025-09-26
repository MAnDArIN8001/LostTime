using System.IO;
using UnityEditor;
using System.Collections.Generic;

namespace CodeGeneration
{
    public static class ConstKeysGenerator
    {
        public static void GenerateItemKeysClass(string className, List<ConstPair> itemsSetups)
        {
            var folderPath = "Assets/Generated";
            var filePath = Path.Combine(folderPath, $"{className}.cs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("// AUTO-GENERATED CODE. DO NOT EDIT.");
                writer.WriteLine("namespace Loot.Data");
                writer.WriteLine("{");
                writer.WriteLine($"    public static class {className}");
                writer.WriteLine("    {");

                foreach (var item in itemsSetups)
                {
                    var safeName = MakeSafeName(item.Name);
                    
                    writer.WriteLine($"        public const string {safeName} = \"{item.Id}\";");
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
        
        private static string MakeSafeName(string name)
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

    public struct ConstPair
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}